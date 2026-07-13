using Flowenter.Api.Extensions;
using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.IServices.Dtos.EnterpriseDto;
using Flowenter.Parties.Mappings;
using Flowenter.Parties.Mappings.Extensions;
using Flowenter.Parties.Models.FacilityModels;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

[ApiController]
[Route("api/parties/enterprises")]
public class EnterprisesController : ControllerBase
{
    private readonly ILogger<EnterprisesController> _logger;
    private readonly IDbContextFactory<PartiesContext> _factory;

    public EnterprisesController(
        ILogger<EnterprisesController> logger,
        IDbContextFactory<PartiesContext> factory)
    {
        _logger = logger;
        _factory = factory;
    }

    [HttpGet]
    [ProducesResponseType(typeof(EnterprisesDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnterprises(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var organizations = await context.PartyRoles
            .Effective()
            .OfType<Enterprise>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var resp = new EnterprisesDto
        {
            Data = organizations.Select(e => e.ToDto()).ToList(),
            TotalCount = await context.PartyRoles.OfType<Enterprise>().CountAsync(cancellationToken),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return Ok(resp);
    }

    [HttpGet("{organization_id:guid}")]
    [ProducesResponseType(typeof(Enterprise), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnterprise([FromRoute] Guid organization_id, CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();
        var organization = await context.PartyRoles.OfType<Enterprise>()
            .FirstOrDefaultAsync(e => e.Id == organization_id, cancellationToken);

        if (organization == null)
        {
            return NotFound();
        }

        return Ok(organization);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Enterprise), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEnterprise(
        [FromBody] CreateEnterpriseDto createDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();
        using var tran = context.Database.BeginTransaction();

        var partyType = await context.PartyTypes
            .FirstOrDefaultAsync(pt => pt.Code == PartyType.Organization, cancellationToken);
        if (partyType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"PartyType '{PartyType.Organization}' not found. Run database migration/seeding.");
        }

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = createDto.LegalName,
            Type = partyType
        };

        await context.AddAsync(org, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var roleType = await context.PartyRoleTypes
            .FirstOrDefaultAsync(pt => pt.Code == PartyRoleType.Enterprise, cancellationToken);
        if (roleType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"PartyRoleType '{PartyRoleType.Enterprise}' not found. Run database migration/seeding.");
        }

        var newData = new Enterprise
        {
            Id = Guid.NewGuid(),
            Type = roleType,
            PartyId = org.Id,

            LegalName = createDto.LegalName,
            Information = createDto.Information,
            BrandName = createDto.BrandName,
            Logo = createDto.Logo,
            Notes = createDto.Notes,
            LegalStructureId = createDto.LegalStructureId,
            BusinessRegistrationNumber = createDto.BusinessRegistrationNumber,
            TaxId = createDto.TaxId,
            FiscalYearStartMonth = createDto.FiscalYearStartMonth
        };

        await context.AddAsync(newData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await tran.CommitAsync(cancellationToken);

        _logger.LogInformation("Enterprise created: {PartyRoleId}", newData.Id);

        return CreatedAtAction(nameof(GetEnterprise), new { organization_id = newData.Id }, newData);
    }

    [HttpPatch("{organization_id:guid}")]
    [ProducesResponseType(typeof(Enterprise), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchEnterprise(
        [FromRoute] Guid organization_id,
        [FromBody] JsonPatchDocument<Enterprise> patchDoc,
        CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var organization = await context.PartyRoles.OfType<Enterprise>()
            .FirstOrDefaultAsync(e => e.Id == organization_id, cancellationToken);

        if (organization == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(organization, ModelState);

        foreach (var operation in patchDoc.Operations)
        {
            if (operation.path.StartsWith($"/{nameof(Enterprise.LegalName)}", StringComparison.OrdinalIgnoreCase))
            {
                var party = await context.Parties.OfType<Organization>()
                    .SingleAsync(o => o.Id == organization.PartyId, cancellationToken);
                party.Name = Convert.ToString(operation.value);
            }
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Ok(organization);
    }

    [HttpDelete("{organization_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterprise(
        [FromRoute] Guid organization_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .FirstOrDefaultAsync(item => item.Id == organization_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var enterpriseRoleId = enterprise.Id!.Value;
        var enterprisePartyId = enterprise.PartyId.Value;
        var enterprisePartyRoleTypeId = enterprise.TypeId.Value;

        var relatedEmployments = await context.PartyRelationships
            .OfType<Employment>()
            .Where(item => item.EnterpriseId == enterpriseRoleId || item.EmployeeId == enterpriseRoleId)
            .ToListAsync(cancellationToken);

        var removedEmployeeRoleIds = relatedEmployments
            .Where(item => item.EmployeeId.HasValue)
            .Select(item => item.EmployeeId!.Value)
            .Distinct()
            .ToList();

        var removedEmployeePartyIds = await context.PartyRoles
            .Where(item => item.Id.HasValue && removedEmployeeRoleIds.Contains(item.Id.Value))
            .Select(item => item.PartyId)
            .Where(item => item.HasValue)
            .Select(item => item!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (relatedEmployments.Count > 0)
        {
            context.PartyRelationships.RemoveRange(relatedEmployments);
        }

        var employeeRoles = await context.PartyRoles
            .Where(item => item.Id.HasValue && removedEmployeeRoleIds.Contains(item.Id.Value))
            .ToListAsync(cancellationToken);
        if (employeeRoles.Count > 0)
        {
            context.PartyRoles.RemoveRange(employeeRoles);
        }

        foreach (var partyId in removedEmployeePartyIds)
        {
            var hasRemainingPartyRoles = await context.PartyRoles
                .AnyAsync(item => item.PartyId == partyId
                                  && item.Id.HasValue
                                  && !removedEmployeeRoleIds.Contains(item.Id.Value),
                    cancellationToken);
            if (hasRemainingPartyRoles)
            {
                continue;
            }

            var personNames = await context.PersonNames
                .Where(item => item.PersonId == partyId)
                .ToListAsync(cancellationToken);
            if (personNames.Count > 0)
            {
                context.PersonNames.RemoveRange(personNames);
            }

            var party = await context.Parties
                .FirstOrDefaultAsync(item => item.Id == partyId, cancellationToken);
            if (party != null)
            {
                context.Parties.Remove(party);
            }
        }

        var branchRoleIdsOfEnterpriseParty = await context.PartyRoles
            .OfType<Branch>()
            .Where(item => item.PartyId == enterprisePartyId && item.Id.HasValue)
            .Select(item => item.Id!.Value)
            .ToListAsync(cancellationToken);

        var relatedEnterpriseBranchRelationships = await context.PartyRelationships
            .OfType<EnterpriseBranch>()
            .Where(item => item.EnterpriseId == enterpriseRoleId
                           || (item.BranchId.HasValue && branchRoleIdsOfEnterpriseParty.Contains(item.BranchId.Value)))
            .ToListAsync(cancellationToken);
        if (relatedEnterpriseBranchRelationships.Count > 0)
        {
            context.PartyRelationships.RemoveRange(relatedEnterpriseBranchRelationships);
        }

        var enterpriseFacilityRoles = await context.FacilityRoles
            .Where(item => item.PartyId == enterprisePartyId && item.PartyRoleTypeId == enterprisePartyRoleTypeId)
            .ToListAsync(cancellationToken);

        var affectedFacilityIds = enterpriseFacilityRoles
            .Where(item => item.Id.HasValue)
            .Select(item => item.Id!.Value)
            .Distinct()
            .ToList();

        if (enterpriseFacilityRoles.Count > 0)
        {
            context.FacilityRoles.RemoveRange(enterpriseFacilityRoles);
        }

        var orphanBedIds = new List<Guid>();
        var orphanRoomIds = new List<Guid>();
        var orphanFloorIds = new List<Guid>();
        var orphanBuildingIds = new List<Guid>();
        foreach (var facilityId in affectedFacilityIds)
        {
            var hasOtherFacilityRoleReferences = await context.FacilityRoles
                .AnyAsync(item =>
                    item.Id == facilityId
                    && (item.PartyId != enterprisePartyId || item.PartyRoleTypeId != enterprisePartyRoleTypeId), cancellationToken);

            if (hasOtherFacilityRoleReferences)
            {
                continue;
            }

            var orphanBedId = await context.Facilities
                .OfType<Bed>()
                .Where(item => item.Id == facilityId)
                .Select(item => item.Id!.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (orphanBedId != Guid.Empty)
            {
                orphanBedIds.Add(orphanBedId);
                continue;
            }

            var orphanRoomId = await context.Facilities
                .OfType<Room>()
                .Where(item => item.Id == facilityId)
                .Select(item => item.Id!.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (orphanRoomId != Guid.Empty)
            {
                orphanRoomIds.Add(orphanRoomId);
                continue;
            }

            var orphanFloorId = await context.Facilities
                .OfType<Floor>()
                .Where(item => item.Id == facilityId)
                .Select(item => item.Id!.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (orphanFloorId != Guid.Empty)
            {
                orphanFloorIds.Add(orphanFloorId);
                continue;
            }

            var orphanBuildingId = await context.Facilities
                .OfType<Building>()
                .Where(item => item.Id == facilityId)
                .Select(item => item.Id!.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (orphanBuildingId != Guid.Empty)
            {
                orphanBuildingIds.Add(orphanBuildingId);
            }
        }

        if (orphanBedIds.Count > 0)
        {
            var orphanBeds = await context.Facilities
                .OfType<Bed>()
                .Where(item => item.Id.HasValue && orphanBedIds.Contains(item.Id.Value))
                .ToListAsync(cancellationToken);
            if (orphanBeds.Count > 0)
            {
                context.Facilities.RemoveRange(orphanBeds);
            }
        }

        if (orphanRoomIds.Count > 0)
        {
            var removableRoomIds = new List<Guid>();
            foreach (var roomId in orphanRoomIds.Distinct())
            {
                var hasBedReference = await context.Facilities
                    .OfType<Bed>()
                    .AnyAsync(item => item.RoomId == roomId && (!item.Id.HasValue || !orphanBedIds.Contains(item.Id.Value)),
                        cancellationToken);
                if (hasBedReference)
                {
                    continue;
                }

                var hasPartOfReference = await context.Facilities
                    .AnyAsync(item => item.PartOfId == roomId, cancellationToken);
                if (hasPartOfReference)
                {
                    continue;
                }

                removableRoomIds.Add(roomId);
            }

            if (removableRoomIds.Count > 0)
            {
                var orphanRooms = await context.Facilities
                    .OfType<Room>()
                    .Where(item => item.Id.HasValue && removableRoomIds.Contains(item.Id.Value))
                    .ToListAsync(cancellationToken);
                if (orphanRooms.Count > 0)
                {
                    context.Facilities.RemoveRange(orphanRooms);
                }
            }
        }

        if (orphanFloorIds.Count > 0)
        {
            var removableFloorIds = new List<Guid>();
            foreach (var floorId in orphanFloorIds.Distinct())
            {
                var hasRoomReference = await context.Facilities
                    .OfType<Room>()
                    .AnyAsync(item => item.FloorId == floorId && (!item.Id.HasValue || !orphanRoomIds.Contains(item.Id.Value)),
                        cancellationToken);
                if (hasRoomReference)
                {
                    continue;
                }

                var hasPartOfReference = await context.Facilities
                    .AnyAsync(item => item.PartOfId == floorId, cancellationToken);
                if (hasPartOfReference)
                {
                    continue;
                }

                removableFloorIds.Add(floorId);
            }

            if (removableFloorIds.Count > 0)
            {
                var orphanFloors = await context.Facilities
                    .OfType<Floor>()
                    .Where(item => item.Id.HasValue && removableFloorIds.Contains(item.Id.Value))
                    .ToListAsync(cancellationToken);
                if (orphanFloors.Count > 0)
                {
                    context.Facilities.RemoveRange(orphanFloors);
                }
            }
        }

        if (orphanBuildingIds.Count > 0)
        {
            var removableBuildingIds = new List<Guid>();
            foreach (var buildingId in orphanBuildingIds.Distinct())
            {
                var hasFloorReference = await context.Facilities
                    .OfType<Floor>()
                    .AnyAsync(item => item.BuildingId == buildingId && (!item.Id.HasValue || !orphanFloorIds.Contains(item.Id.Value)),
                        cancellationToken);
                if (hasFloorReference)
                {
                    continue;
                }

                var hasPartOfReference = await context.Facilities
                    .AnyAsync(item => item.PartOfId == buildingId, cancellationToken);
                if (hasPartOfReference)
                {
                    continue;
                }

                removableBuildingIds.Add(buildingId);
            }

            if (removableBuildingIds.Count > 0)
            {
                var orphanBuildings = await context.Facilities
                    .OfType<Building>()
                    .Where(item => item.Id.HasValue && removableBuildingIds.Contains(item.Id.Value))
                    .ToListAsync(cancellationToken);
                if (orphanBuildings.Count > 0)
                {
                    context.Facilities.RemoveRange(orphanBuildings);
                }
            }
        }

        context.PartyRoles.Remove(enterprise);

        var hasOtherPartyRoles = await context.PartyRoles
            .AnyAsync(item => item.PartyId == enterprisePartyId && item.Id != enterpriseRoleId, cancellationToken);
        if (!hasOtherPartyRoles)
        {
            var enterpriseParty = await context.Parties
                .FirstOrDefaultAsync(item => item.Id == enterprisePartyId, cancellationToken);
            if (enterpriseParty != null)
            {
                context.Parties.Remove(enterpriseParty);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("{enterprise_role_id:guid}/employments")]
    [ProducesResponseType(typeof(List<EmploymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnterpriseEmployments(
        [FromRoute] Guid enterprise_role_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterpriseExists = await context.PartyRoles
            .OfType<Enterprise>()
            .AnyAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (!enterpriseExists)
        {
            return NotFound();
        }

        var employmentRows = await (
            from employment in context.PartyRelationships.OfType<Employment>()
            join employeeRole in context.PartyRoles on employment.EmployeeId equals employeeRole.Id
            join roleType in context.PartyRoleTypes on employeeRole.TypeId equals roleType.Id
            join person in context.Parties.OfType<Person>() on employeeRole.PartyId equals person.Id
            where employment.EnterpriseId == enterprise_role_id
            select new
            {
                Employment = employment,
                EmployeeRole = employeeRole,
                RoleType = roleType,
                PersonId = person.Id
            })
            .ToListAsync(cancellationToken);

        var personIds = employmentRows
            .Select(row => row.PersonId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var personNames = await context.PersonNames
            .Where(personName => personName.PersonId.HasValue
                                 && personIds.Contains(personName.PersonId.Value)
                                 && personName.FromDateUtc <= today
                                 && today <= personName.ThruDateUtc)
            .OrderByDescending(personName => personName.FromDateUtc)
            .ToListAsync(cancellationToken);

        var personNameMap = personNames
            .GroupBy(name => name.PersonId!.Value)
            .ToDictionary(group => group.Key, group => group.First());

        var employeeRoleIds = employmentRows
            .Select(row => row.EmployeeRole.Id)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var employeeBranchRows = await (
            from relation in context.PartyRelationships.OfType<BranchEmployment>()
            join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
            join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
            where relation.EmployeeId.HasValue
                  && employeeRoleIds.Contains(relation.EmployeeId.Value)
            select new
            {
                EmployeeRoleId = relation.EmployeeId!.Value,
                BranchId = relation.BranchId!.Value,
                BranchLegalName = branchParty.Name ?? string.Empty,
                FromDate = relation.FromDateUtc,
                ThruDate = relation.ThruDateUtc
            })
            .ToListAsync(cancellationToken);

        var employeeBranchMap = employeeBranchRows
            .GroupBy(row => row.EmployeeRoleId)
            .ToDictionary(
                group => group.Key,
                group => new
                {
                    BranchIds = group.Select(item => item.BranchId).Distinct().ToList(),
                    BranchLegalNames = group
                        .Select(item => item.BranchLegalName)
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .Distinct()
                        .OrderBy(name => name)
                        .ToList(),
                    BranchEmployments = group
                        .GroupBy(item => item.BranchId)
                        .Select(item => item.First())
                        .OrderBy(item => item.BranchLegalName)
                        .Select(item => new EmploymentBranchDto
                        {
                            BranchId = item.BranchId,
                            BranchLegalName = item.BranchLegalName,
                            FromDate = item.FromDate,
                            ThruDate = item.ThruDate
                        })
                        .ToList()
                });

        var result = employmentRows
            .Select(row =>
            {
                var personName = row.PersonId.HasValue && personNameMap.TryGetValue(row.PersonId.Value, out var name)
                    ? name
                    : null;

                var fullName = personName == null
                    ? "-"
                    : string.Join(" ",
                        new[] { personName.FirstName, personName.MiddleName, personName.LastName }
                            .Where(part => !string.IsNullOrWhiteSpace(part)));
                var branchInfo = row.EmployeeRole.Id.HasValue && employeeBranchMap.TryGetValue(row.EmployeeRole.Id.Value, out var info)
                    ? info
                    : null;

                return new EmploymentDto
                {
                    EmploymentId = row.Employment.Id!.Value,
                    EmploymentNumber = row.Employment.Number ?? string.Empty,
                    EmployerId = row.Employment.EnterpriseId!.Value,
                    EmployeePartyRoleId = row.EmployeeRole.Id!.Value,
                    EmployeePartyId = row.PersonId!.Value,
                    BranchIds = branchInfo?.BranchIds ?? [],
                    BranchLegalNames = branchInfo?.BranchLegalNames ?? [],
                    BranchEmployments = branchInfo?.BranchEmployments ?? [],
                    FirstName = personName?.FirstName ?? string.Empty,
                    MiddleName = personName?.MiddleName,
                    LastName = personName?.LastName ?? string.Empty,
                    EmployeeFullName = fullName,
                    PartyRoleTypeId = row.RoleType.Id!.Value,
                    PartyRoleTypeCode = row.RoleType.Code ?? string.Empty,
                    PartyRoleTypeName = row.RoleType.Name ?? string.Empty,
                    FromDate = row.Employment.FromDateUtc,
                    ThruDate = row.Employment.ThruDateUtc,
                    CreatedAtUtc = row.Employment.CreatedAtUtc,
                    UpdatedAtUtc = row.Employment.UpdatedAtUtc,
                    Revision = row.Employment.Revision
                };
            })
            .OrderBy(item => item.EmployeeFullName)
            .ToList();

        return Ok(result);
    }

    [HttpPost("{enterprise_role_id:guid}/employments")]
    [ProducesResponseType(typeof(EmploymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEnterpriseEmployment(
        [FromRoute] Guid enterprise_role_id,
        [FromBody] CreateEmploymentDto createDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();
        using var tran = await context.Database.BeginTransactionAsync(cancellationToken);

        var enterpriseRole = await context.PartyRoles
            .OfType<Enterprise>()
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterpriseRole == null)
        {
            return NotFound();
        }

        var personPartyType = await context.PartyTypes
            .SingleOrDefaultAsync(partyType => partyType.Code == PartyType.Person, cancellationToken);
        if (personPartyType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"PartyType '{PartyType.Person}' not found. Run database migration/seeding.");
        }

        var employmentRelationshipType = await context.PartyRelationshipTypes
            .SingleOrDefaultAsync(type => type.Code == PartyRelationshipType.Employment, cancellationToken);
        if (employmentRelationshipType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"PartyRelationshipType '{PartyRelationshipType.Employment}' not found. Run database migration/seeding.");
        }

        var selectedRoleTypeIds = createDto.PartyRoleTypeIds
            .Distinct()
            .ToList();
        var selectedBranchRoleIds = createDto.BranchIds
            .Distinct()
            .ToList();
        if (selectedRoleTypeIds.Count == 0)
        {
            return BadRequest("At least one PartyRoleType is required.");
        }

        var partyRoleTypes = await context.PartyRoleTypes
            .Where(type => type.Id.HasValue && selectedRoleTypeIds.Contains(type.Id.Value))
            .ToListAsync(cancellationToken);
        if (partyRoleTypes.Count != selectedRoleTypeIds.Count)
        {
            return BadRequest("One or more PartyRoleTypes were not found.");
        }

        var selectedBranches = await (
            from relation in context.PartyRelationships.OfType<EnterpriseBranch>()
            join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
            join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
            where relation.EnterpriseId == enterprise_role_id
                  && relation.BranchId.HasValue
                  && selectedBranchRoleIds.Contains(relation.BranchId.Value)
            select new
            {
                BranchRoleId = relation.BranchId!.Value,
                BranchLegalName = branchParty.Name ?? string.Empty
            })
        .ToListAsync(cancellationToken);
        if (selectedBranches.Count != selectedBranchRoleIds.Count)
        {
            return BadRequest("One or more Branches were not found in this enterprise.");
        }

        var branchEmploymentRelationshipType = default(PartyRelationshipType);
        if (selectedBranchRoleIds.Count > 0)
        {
            branchEmploymentRelationshipType = await context.PartyRelationshipTypes
                .SingleOrDefaultAsync(type => type.Code == PartyRelationshipType.BranchEmployment, cancellationToken);
            if (branchEmploymentRelationshipType == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Missing master data",
                    detail: $"PartyRelationshipType '{PartyRelationshipType.BranchEmployment}' not found. Run database migration/seeding.");
            }
        }

        var language = createDto.LanguageId.HasValue
            ? await context.Languages.SingleOrDefaultAsync(item => item.Id == createDto.LanguageId, cancellationToken)
            : await context.Languages.SingleOrDefaultAsync(item => item.Code == Language.TH, cancellationToken);

        if (language == null)
        {
            language = await context.Languages
                .OrderBy(item => item.CreatedAtUtc)
                .SingleOrDefaultAsync(item => item.Id != null, cancellationToken);
        }

        if (language?.Id == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: "Language not found. Run database migration/seeding.");
        }

        var person = new Person
        {
            Id = Guid.NewGuid(),
            TypeId = personPartyType.Id,
            DateOfBirth = createDto.DateOfBirth
        };

        var personName = new PersonName(person, createDto.FirstName.Trim(), createDto.LastName.Trim(), language)
        {
            Id = Guid.NewGuid(),
            MiddleName = string.IsNullOrWhiteSpace(createDto.MiddleName) ? null : createDto.MiddleName.Trim(),
            LanguageId = language.Id
        };

        var firstPartyRoleType = partyRoleTypes.First();
        var firstEmployeeRole = default(Employee);
        var firstEmployment = default(Employment);
        var employmentNumber = createDto.EmploymentNumber.Trim();

        await context.AddRangeAsync(person, personName);

        foreach (var partyRoleType in partyRoleTypes)
        {
            var employeeRole = new Employee
            {
                Id = Guid.NewGuid(),
                TypeId = partyRoleType.Id,
                PartyId = person.Id
            };

            var employment = new Employment(enterpriseRole.Id!.Value, employeeRole.Id!.Value, employmentRelationshipType.Id!.Value)
            {
                Id = Guid.NewGuid(),
                PartyRelationshipTypeId = employmentRelationshipType.Id,
                EnterpriseId = enterpriseRole.Id,
                EmployeeId = employeeRole.Id,
                Number = employmentNumber
            };

            await context.AddRangeAsync(employeeRole, employment);

            foreach (var selectedBranch in selectedBranches)
            {
                var branchEmployment = new BranchEmployment(
                    selectedBranch.BranchRoleId,
                    employeeRole.Id!.Value,
                    branchEmploymentRelationshipType!.Id!.Value)
                {
                    Id = Guid.NewGuid(),
                    PartyRelationshipTypeId = branchEmploymentRelationshipType.Id,
                    BranchId = selectedBranch.BranchRoleId,
                    EmployeeId = employeeRole.Id
                };
                await context.AddAsync(branchEmployment, cancellationToken);
            }

            if (firstEmployeeRole == null && firstEmployment == null && firstPartyRoleType.Id == partyRoleType.Id)
            {
                firstEmployeeRole = employeeRole;
                firstEmployment = employment;
            }
        }
        await context.SaveChangesAsync(cancellationToken);
        await tran.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Employment(s) created for enterprise {EnterpriseRoleId} with {RoleCount} party role(s)",
            enterprise_role_id,
            partyRoleTypes.Count);

        var fullName = string.Join(" ",
            new[] { personName.FirstName, personName.MiddleName, personName.LastName }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

        var createdBranchRows = firstEmployeeRole?.Id.HasValue == true
            ? await (
                from relation in context.PartyRelationships.OfType<BranchEmployment>()
                join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
                join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
                where relation.EmployeeId == firstEmployeeRole.Id
                select new EmploymentBranchDto
                {
                    BranchId = relation.BranchId!.Value,
                    BranchLegalName = branchParty.Name ?? string.Empty,
                    FromDate = relation.FromDateUtc,
                    ThruDate = relation.ThruDateUtc
                })
            .OrderBy(item => item.BranchLegalName)
            .ToListAsync(cancellationToken)
            : [];

        var response = new EmploymentDto
        {
            EmploymentId = firstEmployment!.Id!.Value,
            EmploymentNumber = firstEmployment.Number ?? string.Empty,
            EmployerId = firstEmployment.EnterpriseId!.Value,
            EmployeePartyRoleId = firstEmployeeRole!.Id!.Value,
            EmployeePartyId = person.Id!.Value,
            BranchIds = selectedBranches.Select(item => item.BranchRoleId).ToList(),
            BranchLegalNames = selectedBranches.Select(item => item.BranchLegalName).Distinct().OrderBy(item => item).ToList(),
            BranchEmployments = createdBranchRows,
            FirstName = personName.FirstName ?? string.Empty,
            MiddleName = personName.MiddleName,
            LastName = personName.LastName ?? string.Empty,
            EmployeeFullName = fullName,
            PartyRoleTypeId = firstPartyRoleType.Id!.Value,
            PartyRoleTypeCode = firstPartyRoleType.Code ?? string.Empty,
            PartyRoleTypeName = firstPartyRoleType.Name ?? string.Empty,
            FromDate = firstEmployment.FromDateUtc,
            ThruDate = firstEmployment.ThruDateUtc,
            CreatedAtUtc = firstEmployment.CreatedAtUtc,
            UpdatedAtUtc = firstEmployment.UpdatedAtUtc,
            Revision = firstEmployment.Revision
        };

        return Created($"/api/parties/enterprises/{enterprise_role_id}/employments/{firstEmployment.Id}", response);
    }

    [HttpPut("{enterprise_role_id:guid}/employments/{employment_id:guid}")]
    [ProducesResponseType(typeof(EmploymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseEmployment(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid employment_id,
        [FromBody] UpdateEmploymentDto updateDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var employment = await context.PartyRelationships
            .OfType<Employment>()
            .FirstOrDefaultAsync(item => item.Id == employment_id && item.EnterpriseId == enterprise_role_id, cancellationToken);
        if (employment == null)
        {
            return NotFound();
        }

        var employeeRole = await context.PartyRoles
            .FirstOrDefaultAsync(item => item.Id == employment.EmployeeId, cancellationToken);
        if (employeeRole?.PartyId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Invalid data",
                detail: $"PartyRole '{employment.EmployeeId}' not found for Employment '{employment_id}'.");
        }

        var person = await context.Parties
            .OfType<Person>()
            .FirstOrDefaultAsync(item => item.Id == employeeRole.PartyId, cancellationToken);
        if (person?.Id == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Invalid data",
                detail: $"Person '{employeeRole.PartyId}' not found for Employment '{employment_id}'.");
        }

        var selectedRoleTypeIds = updateDto.PartyRoleTypeIds
            .Distinct()
            .ToList();
        var employmentNumber = updateDto.EmploymentNumber.Trim();
        var selectedBranchRoleIds = updateDto.BranchIds
            .Distinct()
            .ToList();
        if (selectedRoleTypeIds.Count == 0)
        {
            return BadRequest("At least one PartyRoleType is required.");
        }

        var selectedPartyRoleTypes = await context.PartyRoleTypes
            .Where(type => type.Id.HasValue && selectedRoleTypeIds.Contains(type.Id.Value))
            .ToListAsync(cancellationToken);
        if (selectedPartyRoleTypes.Count != selectedRoleTypeIds.Count)
        {
            return BadRequest("One or more PartyRoleTypes were not found.");
        }

        var selectedBranches = await (
            from relation in context.PartyRelationships.OfType<EnterpriseBranch>()
            join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
            join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
            where relation.EnterpriseId == enterprise_role_id
                  && relation.BranchId.HasValue
                  && selectedBranchRoleIds.Contains(relation.BranchId.Value)
            select new
            {
                BranchRoleId = relation.BranchId!.Value,
                BranchLegalName = branchParty.Name ?? string.Empty
            })
        .ToListAsync(cancellationToken);
        if (selectedBranches.Count != selectedBranchRoleIds.Count)
        {
            return BadRequest("One or more Branches were not found in this enterprise.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var personName = await context.PersonNames
            .Where(item => item.PersonId == person.Id
                           && item.FromDateUtc <= today
                           && today <= item.ThruDateUtc)
            .OrderByDescending(item => item.FromDateUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (personName == null)
        {
            var language = await context.Languages
                .SingleOrDefaultAsync(item => item.Code == Language.TH, cancellationToken);
            if (language == null)
            {
                language = await context.Languages
                    .OrderBy(item => item.CreatedAtUtc)
                    .SingleOrDefaultAsync(item => item.Id != null, cancellationToken);
            }

            if (language?.Id == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Missing master data",
                    detail: "Language not found. Run database migration/seeding.");
            }

            personName = new PersonName(person, updateDto.FirstName.Trim(), updateDto.LastName.Trim(), language)
            {
                Id = Guid.NewGuid(),
                MiddleName = string.IsNullOrWhiteSpace(updateDto.MiddleName) ? null : updateDto.MiddleName.Trim(),
                LanguageId = language.Id
            };

            await context.AddAsync(personName, cancellationToken);
        }
        else
        {
            personName.FirstName = updateDto.FirstName.Trim();
            personName.MiddleName = string.IsNullOrWhiteSpace(updateDto.MiddleName) ? null : updateDto.MiddleName.Trim();
            personName.LastName = updateDto.LastName.Trim();
        }

        var relatedEmployments = await (
            from employmentItem in context.PartyRelationships.OfType<Employment>()
            join role in context.PartyRoles on employmentItem.EmployeeId equals role.Id
            where employmentItem.EnterpriseId == enterprise_role_id
                  && role.PartyId == person.Id
            select new
            {
                Employment = employmentItem,
                EmployeeRole = role
            })
            .ToListAsync(cancellationToken);

        var relatedEmployeeRoleIds = relatedEmployments
            .Where(item => item.EmployeeRole.Id.HasValue)
            .Select(item => item.EmployeeRole.Id!.Value)
            .ToList();

        var existingBranchEmployments = await context.PartyRelationships
            .OfType<BranchEmployment>()
            .Where(item => item.EmployeeId.HasValue && relatedEmployeeRoleIds.Contains(item.EmployeeId.Value))
            .ToListAsync(cancellationToken);

        var existingByRoleTypeId = relatedEmployments
            .Where(item => item.EmployeeRole.TypeId.HasValue)
            .GroupBy(item => item.EmployeeRole.TypeId!.Value)
            .ToDictionary(group => group.Key, group => group.First());

        var selectedRoleTypeSet = selectedRoleTypeIds.ToHashSet();
        var selectedBranchRoleIdSet = selectedBranchRoleIds.ToHashSet();
        var selectedPartyRoleTypeMap = selectedPartyRoleTypes
            .Where(item => item.Id.HasValue)
            .ToDictionary(item => item.Id!.Value, item => item);

        foreach (var relatedEmployment in relatedEmployments)
        {
            var typeId = relatedEmployment.EmployeeRole.TypeId;
            if (!typeId.HasValue || selectedRoleTypeSet.Contains(typeId.Value))
            {
                relatedEmployment.Employment.Number = employmentNumber;

                if (relatedEmployment.EmployeeRole.Id.HasValue)
                {
                    var employeeRoleId = relatedEmployment.EmployeeRole.Id.Value;
                    var currentBranchEmployments = existingBranchEmployments
                        .Where(item => item.EmployeeId == employeeRoleId)
                        .ToList();

                    foreach (var currentBranchEmployment in currentBranchEmployments)
                    {
                        if (!currentBranchEmployment.BranchId.HasValue || selectedBranchRoleIdSet.Contains(currentBranchEmployment.BranchId.Value))
                        {
                            continue;
                        }

                        context.PartyRelationships.Remove(currentBranchEmployment);
                    }
                }
                continue;
            }

            if (relatedEmployment.EmployeeRole.Id.HasValue)
            {
                var employeeRoleId = relatedEmployment.EmployeeRole.Id.Value;
                var branchEmploymentsToRemove = existingBranchEmployments
                    .Where(item => item.EmployeeId == employeeRoleId)
                    .ToList();
                if (branchEmploymentsToRemove.Count > 0)
                {
                    context.PartyRelationships.RemoveRange(branchEmploymentsToRemove);
                }
            }

            context.PartyRelationships.Remove(relatedEmployment.Employment);
            context.PartyRoles.Remove(relatedEmployment.EmployeeRole);
        }

        var branchEmploymentRelationshipType = default(PartyRelationshipType);
        if (selectedBranchRoleIds.Count > 0)
        {
            branchEmploymentRelationshipType = await context.PartyRelationshipTypes
                .SingleOrDefaultAsync(type => type.Code == PartyRelationshipType.BranchEmployment, cancellationToken);
            if (branchEmploymentRelationshipType == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Missing master data",
                    detail: $"PartyRelationshipType '{PartyRelationshipType.BranchEmployment}' not found. Run database migration/seeding.");
            }
        }

        foreach (var relatedEmployment in relatedEmployments)
        {
            if (!relatedEmployment.EmployeeRole.Id.HasValue)
            {
                continue;
            }

            var employeeRoleId = relatedEmployment.EmployeeRole.Id.Value;
            var roleTypeId = relatedEmployment.EmployeeRole.TypeId;
            if (!roleTypeId.HasValue || !selectedRoleTypeSet.Contains(roleTypeId.Value))
            {
                continue;
            }

            var currentBranchIds = existingBranchEmployments
                .Where(item => item.EmployeeId == employeeRoleId && item.BranchId.HasValue)
                .Select(item => item.BranchId!.Value)
                .ToHashSet();

            foreach (var selectedBranch in selectedBranches)
            {
                if (currentBranchIds.Contains(selectedBranch.BranchRoleId))
                {
                    continue;
                }

                var branchEmploymentToAdd = new BranchEmployment(
                    selectedBranch.BranchRoleId,
                    employeeRoleId,
                    branchEmploymentRelationshipType!.Id!.Value)
                {
                    Id = Guid.NewGuid(),
                    PartyRelationshipTypeId = branchEmploymentRelationshipType.Id,
                    BranchId = selectedBranch.BranchRoleId,
                    EmployeeId = employeeRoleId
                };
                await context.AddAsync(branchEmploymentToAdd, cancellationToken);
            }
        }

        var employmentRelationshipType = await context.PartyRelationshipTypes
            .SingleOrDefaultAsync(type => type.Code == PartyRelationshipType.Employment, cancellationToken);
        if (employmentRelationshipType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"PartyRelationshipType '{PartyRelationshipType.Employment}' not found. Run database migration/seeding.");
        }

        var createdEmployments = new Dictionary<Guid, (Employment Employment, Employee EmployeeRole)>();
        foreach (var selectedRoleTypeId in selectedRoleTypeIds)
        {
            if (existingByRoleTypeId.ContainsKey(selectedRoleTypeId))
            {
                continue;
            }

            var employeeRoleToAdd = new Employee
            {
                Id = Guid.NewGuid(),
                TypeId = selectedRoleTypeId,
                PartyId = person.Id
            };

            var employmentToAdd = new Employment(enterprise_role_id, employeeRoleToAdd.Id!.Value, employmentRelationshipType.Id!.Value)
            {
                Id = Guid.NewGuid(),
                PartyRelationshipTypeId = employmentRelationshipType.Id,
                EnterpriseId = enterprise_role_id,
                EmployeeId = employeeRoleToAdd.Id,
                Number = employmentNumber
            };

            await context.AddRangeAsync(employeeRoleToAdd, employmentToAdd);

            foreach (var selectedBranch in selectedBranches)
            {
                var branchEmploymentToAdd = new BranchEmployment(
                    selectedBranch.BranchRoleId,
                    employeeRoleToAdd.Id!.Value,
                    branchEmploymentRelationshipType!.Id!.Value)
                {
                    Id = Guid.NewGuid(),
                    PartyRelationshipTypeId = branchEmploymentRelationshipType.Id,
                    BranchId = selectedBranch.BranchRoleId,
                    EmployeeId = employeeRoleToAdd.Id
                };
                await context.AddAsync(branchEmploymentToAdd, cancellationToken);
            }
            createdEmployments[selectedRoleTypeId] = (employmentToAdd, employeeRoleToAdd);
        }

        await context.SaveChangesAsync(cancellationToken);

        var fullName = string.Join(" ",
            new[] { personName.FirstName, personName.MiddleName, personName.LastName }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

        var primaryRoleTypeId = selectedRoleTypeIds.First();
        var primaryRoleType = selectedPartyRoleTypeMap[primaryRoleTypeId];
        var selectedExisting = existingByRoleTypeId.TryGetValue(primaryRoleTypeId, out var existingItem)
            ? existingItem
            : null;
        var selectedCreated = createdEmployments.TryGetValue(primaryRoleTypeId, out var createdItem)
            ? createdItem
            : default;

        var responseEmployment = selectedExisting?.Employment ?? selectedCreated.Employment;
        var responseEmployeeRoleId = selectedExisting?.EmployeeRole.Id ?? selectedCreated.EmployeeRole.Id;
        var updatedBranchRows = responseEmployeeRoleId.HasValue
            ? await (
                from relation in context.PartyRelationships.OfType<BranchEmployment>()
                join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
                join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
                where relation.EmployeeId == responseEmployeeRoleId
                select new EmploymentBranchDto
                {
                    BranchId = relation.BranchId!.Value,
                    BranchLegalName = branchParty.Name ?? string.Empty,
                    FromDate = relation.FromDateUtc,
                    ThruDate = relation.ThruDateUtc
                })
            .OrderBy(item => item.BranchLegalName)
            .ToListAsync(cancellationToken)
            : [];

        return Ok(new EmploymentDto
        {
            EmploymentId = responseEmployment.Id!.Value,
            EmploymentNumber = responseEmployment.Number ?? string.Empty,
            EmployerId = responseEmployment.EnterpriseId.Value,
            EmployeePartyRoleId = responseEmployeeRoleId!.Value,
            EmployeePartyId = person.Id!.Value,
            BranchIds = selectedBranches.Select(item => item.BranchRoleId).ToList(),
            BranchLegalNames = selectedBranches.Select(item => item.BranchLegalName).Distinct().OrderBy(item => item).ToList(),
            BranchEmployments = updatedBranchRows,
            FirstName = personName.FirstName ?? string.Empty,
            MiddleName = personName.MiddleName,
            LastName = personName.LastName ?? string.Empty,
            EmployeeFullName = fullName,
            PartyRoleTypeId = primaryRoleType.Id!.Value,
            PartyRoleTypeCode = primaryRoleType.Code ?? string.Empty,
            PartyRoleTypeName = primaryRoleType.Name ?? string.Empty,
            FromDate = responseEmployment.FromDateUtc,
            ThruDate = responseEmployment.ThruDateUtc,
            CreatedAtUtc = responseEmployment.CreatedAtUtc,
            UpdatedAtUtc = responseEmployment.UpdatedAtUtc,
            Revision = responseEmployment.Revision
        });
    }

    [HttpPatch("{enterprise_role_id:guid}/employments/{employment_id:guid}/effective-date")]
    [ProducesResponseType(typeof(EmploymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseEmploymentEffectiveDate(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid employment_id,
        [FromBody] UpdateEmploymentEffectiveDateDto updateDto,
        CancellationToken cancellationToken)
    {
        if (updateDto.FromDate > updateDto.ThruDate)
        {
            return BadRequest("FromDate must be earlier than or equal to ThruDate.");
        }

        using var context = _factory.CreateDbContext();

        var employmentRow = await (
            from employment in context.PartyRelationships.OfType<Employment>()
            join employeeRole in context.PartyRoles on employment.EmployeeId equals employeeRole.Id
            join roleType in context.PartyRoleTypes on employeeRole.TypeId equals roleType.Id
            where employment.Id == employment_id && employment.EnterpriseId == enterprise_role_id
            select new
            {
                Employment = employment,
                EmployeeRole = employeeRole,
                RoleType = roleType
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (employmentRow == null || employmentRow.EmployeeRole.PartyId == null)
        {
            return NotFound();
        }

        employmentRow.Employment.FromDateUtc = updateDto.FromDate;
        employmentRow.Employment.ThruDateUtc = updateDto.ThruDate;
        await context.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var personName = await context.PersonNames
            .Where(item => item.PersonId == employmentRow.EmployeeRole.PartyId
                           && item.FromDateUtc <= today
                           && today <= item.ThruDateUtc)
            .OrderByDescending(item => item.FromDateUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var fullName = personName == null
            ? "-"
            : string.Join(" ", new[] { personName.FirstName, personName.MiddleName, personName.LastName }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

        var branchRows = await (
            from relation in context.PartyRelationships.OfType<BranchEmployment>()
            join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
            join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
            where relation.EmployeeId == employmentRow.EmployeeRole.Id
            select new EmploymentBranchDto
            {
                BranchId = relation.BranchId!.Value,
                BranchLegalName = branchParty.Name ?? string.Empty,
                FromDate = relation.FromDateUtc,
                ThruDate = relation.ThruDateUtc
            })
            .OrderBy(item => item.BranchLegalName)
            .ToListAsync(cancellationToken);

        return Ok(new EmploymentDto
        {
            EmploymentId = employmentRow.Employment.Id!.Value,
            EmploymentNumber = employmentRow.Employment.Number ?? string.Empty,
            EmployerId = employmentRow.Employment.EnterpriseId!.Value,
            EmployeePartyRoleId = employmentRow.EmployeeRole.Id!.Value,
            EmployeePartyId = employmentRow.EmployeeRole.PartyId!.Value,
            BranchIds = branchRows.Select(item => item.BranchId).Distinct().ToList(),
            BranchLegalNames = branchRows.Select(item => item.BranchLegalName).Where(item => !string.IsNullOrWhiteSpace(item)).Distinct().OrderBy(item => item).ToList(),
            BranchEmployments = branchRows,
            FirstName = personName?.FirstName ?? string.Empty,
            MiddleName = personName?.MiddleName,
            LastName = personName?.LastName ?? string.Empty,
            EmployeeFullName = fullName,
            PartyRoleTypeId = employmentRow.RoleType.Id!.Value,
            PartyRoleTypeCode = employmentRow.RoleType.Code ?? string.Empty,
            PartyRoleTypeName = employmentRow.RoleType.Name ?? string.Empty,
            FromDate = employmentRow.Employment.FromDateUtc,
            ThruDate = employmentRow.Employment.ThruDateUtc,
            CreatedAtUtc = employmentRow.Employment.CreatedAtUtc,
            UpdatedAtUtc = employmentRow.Employment.UpdatedAtUtc,
            Revision = employmentRow.Employment.Revision
        });
    }

    [HttpPatch("{enterprise_role_id:guid}/employments/{employment_id:guid}/branches/{branch_id:guid}/effective-date")]
    [ProducesResponseType(typeof(EmploymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseEmploymentBranchEffectiveDate(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid employment_id,
        [FromRoute] Guid branch_id,
        [FromBody] UpdateEmploymentEffectiveDateDto updateDto,
        CancellationToken cancellationToken)
    {
        if (updateDto.FromDate > updateDto.ThruDate)
        {
            return BadRequest("FromDate must be earlier than or equal to ThruDate.");
        }

        using var context = _factory.CreateDbContext();

        var employmentRow = await (
            from employment in context.PartyRelationships.OfType<Employment>()
            join employeeRole in context.PartyRoles on employment.EmployeeId equals employeeRole.Id
            join roleType in context.PartyRoleTypes on employeeRole.TypeId equals roleType.Id
            where employment.Id == employment_id && employment.EnterpriseId == enterprise_role_id
            select new
            {
                Employment = employment,
                EmployeeRole = employeeRole,
                RoleType = roleType
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (employmentRow == null || employmentRow.EmployeeRole.PartyId == null)
        {
            return NotFound();
        }

        var relatedEmployeeRoleIds = await (
            from employment in context.PartyRelationships.OfType<Employment>()
            join employeeRole in context.PartyRoles on employment.EmployeeId equals employeeRole.Id
            where employment.EnterpriseId == enterprise_role_id
                  && employeeRole.PartyId == employmentRow.EmployeeRole.PartyId
                  && employeeRole.Id.HasValue
            select employeeRole.Id!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        if (relatedEmployeeRoleIds.Count == 0)
        {
            return NotFound();
        }

        var branchEmployments = await context.PartyRelationships
            .OfType<BranchEmployment>()
            .Where(item => item.EmployeeId.HasValue
                           && relatedEmployeeRoleIds.Contains(item.EmployeeId.Value)
                           && item.BranchId == branch_id)
            .ToListAsync(cancellationToken);
        if (branchEmployments.Count == 0)
        {
            return NotFound();
        }

        foreach (var branchEmployment in branchEmployments)
        {
            branchEmployment.FromDateUtc = updateDto.FromDate;
            branchEmployment.ThruDateUtc = updateDto.ThruDate;
        }

        await context.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var personName = await context.PersonNames
            .Where(item => item.PersonId == employmentRow.EmployeeRole.PartyId
                           && item.FromDateUtc <= today
                           && today <= item.ThruDateUtc)
            .OrderByDescending(item => item.FromDateUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var fullName = personName == null
            ? "-"
            : string.Join(" ", new[] { personName.FirstName, personName.MiddleName, personName.LastName }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

        var branchRows = await (
            from relation in context.PartyRelationships.OfType<BranchEmployment>()
            join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
            join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
            where relation.EmployeeId == employmentRow.EmployeeRole.Id
            select new EmploymentBranchDto
            {
                BranchId = relation.BranchId!.Value,
                BranchLegalName = branchParty.Name ?? string.Empty,
                FromDate = relation.FromDateUtc,
                ThruDate = relation.ThruDateUtc
            })
            .OrderBy(item => item.BranchLegalName)
            .ToListAsync(cancellationToken);

        return Ok(new EmploymentDto
        {
            EmploymentId = employmentRow.Employment.Id!.Value,
            EmploymentNumber = employmentRow.Employment.Number ?? string.Empty,
            EmployerId = employmentRow.Employment.EnterpriseId!.Value,
            EmployeePartyRoleId = employmentRow.EmployeeRole.Id!.Value,
            EmployeePartyId = employmentRow.EmployeeRole.PartyId!.Value,
            BranchIds = branchRows.Select(item => item.BranchId).Distinct().ToList(),
            BranchLegalNames = branchRows.Select(item => item.BranchLegalName).Where(item => !string.IsNullOrWhiteSpace(item)).Distinct().OrderBy(item => item).ToList(),
            BranchEmployments = branchRows,
            FirstName = personName?.FirstName ?? string.Empty,
            MiddleName = personName?.MiddleName,
            LastName = personName?.LastName ?? string.Empty,
            EmployeeFullName = fullName,
            PartyRoleTypeId = employmentRow.RoleType.Id!.Value,
            PartyRoleTypeCode = employmentRow.RoleType.Code ?? string.Empty,
            PartyRoleTypeName = employmentRow.RoleType.Name ?? string.Empty,
            FromDate = employmentRow.Employment.FromDateUtc,
            ThruDate = employmentRow.Employment.ThruDateUtc,
            CreatedAtUtc = employmentRow.Employment.CreatedAtUtc,
            UpdatedAtUtc = employmentRow.Employment.UpdatedAtUtc,
            Revision = employmentRow.Employment.Revision
        });
    }

    [HttpGet("{enterprise_role_id:guid}/branchs")]
    [ProducesResponseType(typeof(List<EnterpriseBranchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnterpriseBranchs(
        [FromRoute] Guid enterprise_role_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterpriseExists = await context.PartyRoles
            .OfType<Enterprise>()
            .AnyAsync(item => item.Id == enterprise_role_id, cancellationToken);
        if (!enterpriseExists)
        {
            return NotFound();
        }

        var branchs = await (
            from relation in context.PartyRelationships.OfType<EnterpriseBranch>()
            join branchRole in context.PartyRoles.OfType<Branch>() on relation.BranchId equals branchRole.Id
            join branchParty in context.Parties.OfType<Organization>() on branchRole.PartyId equals branchParty.Id
            where relation.EnterpriseId == enterprise_role_id
            orderby branchParty.Name
            select new EnterpriseBranchDto
            {
                EnterpriseBranchId = relation.Id!.Value,
                EnterpriseId = relation.EnterpriseId!.Value,
                BranchId = relation.BranchId!.Value,
                BranchLegalName = branchParty.Name ?? string.Empty,
                CreatedAtUtc = relation.CreatedAtUtc,
                UpdatedAtUtc = relation.UpdatedAtUtc,
                Revision = relation.Revision
            })
            .ToListAsync(cancellationToken);

        return Ok(branchs);
    }

    [HttpPost("{enterprise_role_id:guid}/branchs")]
    [ProducesResponseType(typeof(EnterpriseBranchDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEnterpriseBranch(
        [FromRoute] Guid enterprise_role_id,
        [FromBody] CreateEnterpriseBranchDto createDto,
        CancellationToken cancellationToken)
    {
        var branchName = createDto.Name.Trim();
        if (string.IsNullOrWhiteSpace(branchName))
        {
            return BadRequest("Branch name is required.");
        }

        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(item => new { item.Id, item.PartyId })
            .FirstOrDefaultAsync(item => item.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.Id.HasValue)
        {
            return NotFound();
        }

        var branchRoleType = await context.PartyRoleTypes
            .SingleOrDefaultAsync(item => item.Code == PartyRoleType.Branch, cancellationToken);
        if (branchRoleType == null)
        {
            branchRoleType = new PartyRoleType
            {
                Id = Guid.NewGuid(),
                Code = PartyRoleType.Branch,
                Name = "สาขา"
            };
            await context.AddAsync(branchRoleType, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var enterpriseBranchRelationshipType = await context.PartyRelationshipTypes
            .SingleOrDefaultAsync(item => item.Code == PartyRelationshipType.EnterpriseBranch, cancellationToken);
        if (enterpriseBranchRelationshipType == null)
        {
            enterpriseBranchRelationshipType = new PartyRelationshipType
            {
                Id = Guid.NewGuid(),
                Code = PartyRelationshipType.EnterpriseBranch,
                Name = "สาขา"
            };
            await context.AddAsync(enterpriseBranchRelationshipType, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var organizationPartyType = await context.PartyTypes
            .SingleOrDefaultAsync(item => item.Code == PartyType.Organization, cancellationToken);
        if (organizationPartyType == null)
        {
            organizationPartyType = new PartyType
            {
                Id = Guid.NewGuid(),
                Code = PartyType.Organization,
                Name = "Organization"
            };
            await context.AddAsync(organizationPartyType, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var branchOrganization = await context.Parties
            .OfType<Organization>()
            .FirstOrDefaultAsync(item => item.Name == branchName, cancellationToken);
        if (branchOrganization == null)
        {
            branchOrganization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = branchName,
                TypeId = organizationPartyType.Id
            };
            await context.AddAsync(branchOrganization, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var branchRole = await context.PartyRoles
            .OfType<Branch>()
            .FirstOrDefaultAsync(item => item.PartyId == branchOrganization.Id && item.TypeId == branchRoleType.Id, cancellationToken);
        if (branchRole == null)
        {
            branchRole = new Branch
            {
                Id = Guid.NewGuid(),
                PartyId = branchOrganization.Id,
                TypeId = branchRoleType.Id,
            };
            await context.AddAsync(branchRole, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var relationExists = await context.PartyRelationships
            .OfType<EnterpriseBranch>()
            .AnyAsync(item => item.EnterpriseId == enterprise.Id
                              && item.BranchId == branchRole.Id,
                cancellationToken);
        if (relationExists)
        {
            return BadRequest("Branch already exists for this enterprise.");
        }

        var relation = new EnterpriseBranch(enterprise.Id.Value, branchRole.Id!.Value, enterpriseBranchRelationshipType.Id!.Value)
        {
            Id = Guid.NewGuid()
        };

        await context.AddAsync(relation, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Created($"/api/parties/enterprises/{enterprise_role_id}/branchs/{relation.Id}", new EnterpriseBranchDto
        {
            EnterpriseBranchId = relation.Id!.Value,
            EnterpriseId = relation.EnterpriseId!.Value,
            BranchId = relation.BranchId!.Value,
            BranchLegalName = branchOrganization.Name ?? string.Empty,
            CreatedAtUtc = relation.CreatedAtUtc,
            UpdatedAtUtc = relation.UpdatedAtUtc,
            Revision = relation.Revision
        });
    }

    [HttpPut("{enterprise_role_id:guid}/branchs/{enterprise_branch_id:guid}")]
    [ProducesResponseType(typeof(EnterpriseBranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseBranch(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid enterprise_branch_id,
        [FromBody] UpdateEnterpriseBranchDto updateDto,
        CancellationToken cancellationToken)
    {
        var branchName = updateDto.Name.Trim();
        if (string.IsNullOrWhiteSpace(branchName))
        {
            return BadRequest("Branch name is required.");
        }

        using var context = _factory.CreateDbContext();

        var relation = await context.PartyRelationships
            .OfType<EnterpriseBranch>()
            .Include(item => item.Branch)
            .ThenInclude(item => item!.Party)
            .FirstOrDefaultAsync(item => item.Id == enterprise_branch_id && item.EnterpriseId == enterprise_role_id, cancellationToken);
        if (relation == null)
        {
            return NotFound();
        }

        var branchRoleType = await context.PartyRoleTypes
            .SingleOrDefaultAsync(item => item.Code == PartyRoleType.Branch, cancellationToken);
        if (branchRoleType == null)
        {
            branchRoleType = new PartyRoleType
            {
                Id = Guid.NewGuid(),
                Code = PartyRoleType.Branch,
                Name = "สาขา"
            };
            await context.AddAsync(branchRoleType, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var organizationPartyType = await context.PartyTypes
            .SingleOrDefaultAsync(item => item.Code == PartyType.Organization, cancellationToken);
        if (organizationPartyType == null)
        {
            organizationPartyType = new PartyType
            {
                Id = Guid.NewGuid(),
                Code = PartyType.Organization,
                Name = "Organization"
            };
            await context.AddAsync(organizationPartyType, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var branchOrganization = await context.Parties
            .OfType<Organization>()
            .FirstOrDefaultAsync(item => item.Name == branchName, cancellationToken);
        if (branchOrganization == null)
        {
            var currentBranchOrganization = relation.Branch?.Party as Organization;
            if (currentBranchOrganization != null)
            {
                currentBranchOrganization.Name = branchName;
                await context.SaveChangesAsync(cancellationToken);
                branchOrganization = currentBranchOrganization;
            }
            else
            {
                branchOrganization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = branchName,
                    TypeId = organizationPartyType.Id
                };
                await context.AddAsync(branchOrganization, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        var branchRole = await context.PartyRoles
            .OfType<Branch>()
            .FirstOrDefaultAsync(item => item.PartyId == branchOrganization.Id && item.TypeId == branchRoleType.Id, cancellationToken);
        if (branchRole == null)
        {
            branchRole = new Branch
            {
                Id = Guid.NewGuid(),
                PartyId = branchOrganization.Id,
                TypeId = branchRoleType.Id,
            };
            await context.AddAsync(branchRole, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var relationExists = await context.PartyRelationships
            .OfType<EnterpriseBranch>()
            .AnyAsync(item => item.Id != enterprise_branch_id
                              && item.EnterpriseId == enterprise_role_id
                              && item.BranchId == branchRole.Id,
                cancellationToken);
        if (relationExists)
        {
            return BadRequest("Branch already exists for this enterprise.");
        }

        relation.BranchId = branchRole.Id;
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new EnterpriseBranchDto
        {
            EnterpriseBranchId = relation.Id!.Value,
            EnterpriseId = relation.EnterpriseId!.Value,
            BranchId = relation.BranchId!.Value,
            BranchLegalName = branchOrganization.Name ?? string.Empty,
            CreatedAtUtc = relation.CreatedAtUtc,
            UpdatedAtUtc = relation.UpdatedAtUtc,
            Revision = relation.Revision
        });
    }

    [HttpDelete("{enterprise_role_id:guid}/branchs/{enterprise_branch_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterpriseBranch(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid enterprise_branch_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var relation = await context.PartyRelationships
            .OfType<EnterpriseBranch>()
            .FirstOrDefaultAsync(item => item.Id == enterprise_branch_id && item.EnterpriseId == enterprise_role_id, cancellationToken);
        if (relation == null)
        {
            return NotFound();
        }

        var branchRoleId = relation.BranchId;

        if (branchRoleId.HasValue)
        {
            var enterpriseEmployeeRoleIds = await context.PartyRelationships
                .OfType<Employment>()
                .Where(item => item.EnterpriseId == enterprise_role_id && item.EmployeeId.HasValue)
                .Select(item => item.EmployeeId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (enterpriseEmployeeRoleIds.Count > 0)
            {
                var branchEmployments = await context.PartyRelationships
                    .OfType<BranchEmployment>()
                    .Where(item => item.BranchId == branchRoleId
                                   && item.EmployeeId.HasValue
                                   && enterpriseEmployeeRoleIds.Contains(item.EmployeeId.Value))
                    .ToListAsync(cancellationToken);
                if (branchEmployments.Count > 0)
                {
                    context.PartyRelationships.RemoveRange(branchEmployments);
                }
            }
        }

        context.PartyRelationships.Remove(relation);
        await context.SaveChangesAsync(cancellationToken);

        if (branchRoleId.HasValue)
        {
            var hasOtherReference = await context.PartyRelationships
                .OfType<EnterpriseBranch>()
                .AnyAsync(item => item.BranchId == branchRoleId, cancellationToken);
            var hasBranchEmploymentReference = await context.PartyRelationships
                .OfType<BranchEmployment>()
                .AnyAsync(item => item.BranchId == branchRoleId, cancellationToken);
            if (!hasOtherReference && !hasBranchEmploymentReference)
            {
                var branchRole = await context.PartyRoles
                    .OfType<Branch>()
                    .FirstOrDefaultAsync(item => item.Id == branchRoleId, cancellationToken);
                if (branchRole != null)
                {
                    context.PartyRoles.Remove(branchRole);
                    await context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        return NoContent();
    }

    [HttpDelete("{enterprise_role_id:guid}/employments/{employment_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterpriseEmployment(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid employment_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var employment = await context.PartyRelationships
            .OfType<Employment>()
            .FirstOrDefaultAsync(item => item.Id == employment_id && item.EnterpriseId == enterprise_role_id, cancellationToken);
        if (employment == null)
        {
            return NotFound();
        }

        var employeeRole = await context.PartyRoles
            .FirstOrDefaultAsync(item => item.Id == employment.EmployeeId, cancellationToken);
        if (employeeRole?.PartyId == null)
        {
            if (employment.EmployeeId.HasValue)
            {
                var orphanBranchEmployments = await context.PartyRelationships
                    .OfType<BranchEmployment>()
                    .Where(item => item.EmployeeId == employment.EmployeeId)
                    .ToListAsync(cancellationToken);
                if (orphanBranchEmployments.Count > 0)
                {
                    context.PartyRelationships.RemoveRange(orphanBranchEmployments);
                }
            }

            context.PartyRelationships.Remove(employment);
            await context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        var relatedEmployments = await (
            from employmentItem in context.PartyRelationships.OfType<Employment>()
            join role in context.PartyRoles on employmentItem.EmployeeId equals role.Id
            where employmentItem.EnterpriseId == enterprise_role_id
                  && role.PartyId == employeeRole.PartyId
            select new
            {
                Employment = employmentItem,
                EmployeeRole = role
            })
            .ToListAsync(cancellationToken);

        foreach (var relatedEmployment in relatedEmployments)
        {
            context.PartyRelationships.Remove(relatedEmployment.Employment);
            context.PartyRoles.Remove(relatedEmployment.EmployeeRole);
        }

        var partyId = employeeRole.PartyId.Value;
        var removedRoleIds = relatedEmployments
            .Where(item => item.EmployeeRole.Id.HasValue)
            .Select(item => item.EmployeeRole.Id!.Value)
            .ToList();

        if (removedRoleIds.Count > 0)
        {
            var relatedBranchEmployments = await context.PartyRelationships
                .OfType<BranchEmployment>()
                .Where(item => item.EmployeeId.HasValue && removedRoleIds.Contains(item.EmployeeId.Value))
                .ToListAsync(cancellationToken);
            if (relatedBranchEmployments.Count > 0)
            {
                context.PartyRelationships.RemoveRange(relatedBranchEmployments);
            }
        }

        var hasRemainingPartyRoles = await context.PartyRoles
            .AnyAsync(item => item.PartyId == partyId
                              && item.Id.HasValue
                              && !removedRoleIds.Contains(item.Id.Value),
                cancellationToken);

        if (!hasRemainingPartyRoles)
        {
            var personNames = await context.PersonNames
                .Where(item => item.PersonId == partyId)
                .ToListAsync(cancellationToken);
            if (personNames.Count > 0)
            {
                context.PersonNames.RemoveRange(personNames);
            }

            var party = await context.Parties
                .FirstOrDefaultAsync(item => item.Id == partyId, cancellationToken);
            if (party != null)
            {
                context.Parties.Remove(party);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{enterprise_role_id:guid}/facilities/tree")]
    [ProducesResponseType(typeof(EnterpriseFacilitiesTreeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnterpriseFacilitiesTree(
        [FromRoute] Guid enterprise_role_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var facilityIds = await context.FacilityRoles
            .Where(role =>
                role.Id.HasValue
                && role.PartyId == enterprise.PartyId
                && role.PartyRoleTypeId == enterprise.TypeId)
            .Select(role => role.Id!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (facilityIds.Count == 0)
        {
            return Ok(new EnterpriseFacilitiesTreeDto());
        }

        var buildings = await context.Facilities
            .OfType<Building>()
            .Where(item => item.Id.HasValue && facilityIds.Contains(item.Id.Value))
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        var floors = await context.Facilities
            .OfType<Floor>()
            .Include(item => item.Building)
            .Where(item => item.Id.HasValue && facilityIds.Contains(item.Id.Value))
            .OrderBy(item => item.Level)
            .ToListAsync(cancellationToken);

        var rooms = await context.Facilities
            .OfType<Room>()
            .Include(item => item.Floor)
            .ThenInclude(item => item!.Building)
            .Where(item => item.Id.HasValue && facilityIds.Contains(item.Id.Value))
            .OrderBy(item => item.Number)
            .ToListAsync(cancellationToken);

        var beds = await context.Facilities
            .OfType<Bed>()
            .Include(item => item.Room)
            .ThenInclude(item => item!.Floor)
            .ThenInclude(item => item!.Building)
            .Where(item => item.Id.HasValue && facilityIds.Contains(item.Id.Value))
            .OrderBy(item => item.Number)
            .ToListAsync(cancellationToken);

        var bedsByRoomId = beds
            .Where(item => item.RoomId.HasValue)
            .GroupBy(item => item.RoomId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        var roomsByFloorId = rooms
            .Where(item => item.FloorId.HasValue)
            .GroupBy(item => item.FloorId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        var floorsByBuildingId = floors
            .Where(item => item.BuildingId.HasValue)
            .GroupBy(item => item.BuildingId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        var tree = new EnterpriseFacilitiesTreeDto
        {
            Buildings = buildings.Select(building =>
            {
                var buildingId = building.Id!.Value;
                var floorNodes = floorsByBuildingId.TryGetValue(buildingId, out var relatedFloors)
                    ? relatedFloors.Select(floor =>
                    {
                        var floorId = floor.Id!.Value;
                        var roomNodes = roomsByFloorId.TryGetValue(floorId, out var relatedRooms)
                            ? relatedRooms.Select(room =>
                            {
                                var roomId = room.Id!.Value;
                                var roomBeds = bedsByRoomId.TryGetValue(roomId, out var relatedBeds)
                                    ? relatedBeds
                                    : [];
                                return new RoomFacilitiesNodeDto
                                {
                                    Room = new RoomDto
                                    {
                                        RoomId = roomId,
                                        Number = room.Number ?? string.Empty,
                                        Description = room.Description,
                                        FloorId = floorId,
                                        FloorLevel = floor.Level,
                                        BuildingId = buildingId,
                                        BuildingName = building.Name ?? string.Empty,
                                        BedCount = roomBeds.Count,
                                        CreatedAtUtc = room.CreatedAtUtc,
                                        UpdatedAtUtc = room.UpdatedAtUtc,
                                        Revision = room.Revision
                                    },
                                    Beds = roomBeds.Select(bed => new BedDto
                                    {
                                        BedId = bed.Id!.Value,
                                        Number = bed.Number ?? string.Empty,
                                        Description = bed.Description,
                                        RoomId = roomId,
                                        RoomNumber = room.Number ?? string.Empty,
                                        FloorId = floorId,
                                        FloorLevel = floor.Level,
                                        BuildingId = buildingId,
                                        BuildingName = building.Name,
                                        CreatedAtUtc = bed.CreatedAtUtc,
                                        UpdatedAtUtc = bed.UpdatedAtUtc,
                                        Revision = bed.Revision
                                    }).ToList()
                                };
                            }).ToList()
                            : [];

                        return new FloorFacilitiesNodeDto
                        {
                            Floor = new FloorDto
                            {
                                FloorId = floorId,
                                Level = floor.Level,
                                Description = floor.Description,
                                BuildingId = buildingId,
                                BuildingName = building.Name ?? string.Empty,
                                CreatedAtUtc = floor.CreatedAtUtc,
                                UpdatedAtUtc = floor.UpdatedAtUtc,
                                Revision = floor.Revision
                            },
                            Rooms = roomNodes
                        };
                    }).ToList()
                    : [];

                return new BuildingFacilitiesNodeDto
                {
                    Building = new BuildingDto
                    {
                        BuildingId = buildingId,
                        Name = building.Name ?? string.Empty,
                        Description = building.Description,
                        CreatedAtUtc = building.CreatedAtUtc,
                        UpdatedAtUtc = building.UpdatedAtUtc,
                        Revision = building.Revision
                    },
                    Floors = floorNodes
                };
            }).ToList()
        };

        return Ok(tree);
    }

    [HttpPost("{enterprise_role_id:guid}/facilities/buildings")]
    [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEnterpriseBuilding(
        [FromRoute] Guid enterprise_role_id,
        [FromBody] CreateBuildingDto createDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var buildingFacilityType = await context.FacilityTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityType.Building, cancellationToken);
        if (buildingFacilityType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityType '{FacilityType.Building}' not found. Run database migration/seeding.");
        }

        var ownFacilityRoleType = await context.FacilityRoleTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityRoleType.Own, cancellationToken);
        if (ownFacilityRoleType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityRoleType '{FacilityRoleType.Own}' not found. Run database migration/seeding.");
        }

        var building = new Building
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(createDto.Description) ? null : createDto.Description.Trim(),
            FacilityTypeId = buildingFacilityType.Id
        };

        var facilityRole = new FacilityRole
        {
            Id = building.Id,
            FacilityRoleTypeId = ownFacilityRoleType.Id,
            PartyId = enterprise.PartyId,
            PartyRoleTypeId = enterprise.TypeId
        };

        await context.AddAsync(building, cancellationToken);
        await context.AddAsync(facilityRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Created($"/api/parties/enterprises/{enterprise_role_id}/facilities/buildings/{building.Id}", new BuildingDto
        {
            BuildingId = building.Id!.Value,
            Name = building.Name ?? string.Empty,
            Description = building.Description,
            CreatedAtUtc = building.CreatedAtUtc,
            UpdatedAtUtc = building.UpdatedAtUtc,
            Revision = building.Revision
        });
    }

    [HttpPut("{enterprise_role_id:guid}/facilities/buildings/{building_id:guid}")]
    [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseBuilding(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid building_id,
        [FromBody] UpdateBuildingDto updateDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var building = await context.Facilities
            .OfType<Building>()
            .FirstOrDefaultAsync(item =>
                    item.Id == building_id
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (building == null)
        {
            return NotFound();
        }

        building.Name = updateDto.Name.Trim();
        building.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
        await context.SaveChangesAsync(cancellationToken);

        return Ok(new BuildingDto
        {
            BuildingId = building.Id!.Value,
            Name = building.Name ?? string.Empty,
            Description = building.Description,
            CreatedAtUtc = building.CreatedAtUtc,
            UpdatedAtUtc = building.UpdatedAtUtc,
            Revision = building.Revision
        });
    }

    [HttpDelete("{enterprise_role_id:guid}/facilities/buildings/{building_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterpriseBuilding(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid building_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var enterpriseBuildingRoles = await context.FacilityRoles
            .Where(role => role.Id == building_id
                           && role.PartyId == enterprise.PartyId
                           && role.PartyRoleTypeId == enterprise.TypeId)
            .ToListAsync(cancellationToken);
        if (enterpriseBuildingRoles.Count == 0)
        {
            return NotFound();
        }

        context.FacilityRoles.RemoveRange(enterpriseBuildingRoles);

        var hasOtherFacilityRoleReferences = await context.FacilityRoles
            .AnyAsync(role => role.Id == building_id
                              && (role.PartyId != enterprise.PartyId || role.PartyRoleTypeId != enterprise.TypeId),
                cancellationToken);
        if (!hasOtherFacilityRoleReferences)
        {
            var hasFloorReference = await context.Facilities
                .OfType<Floor>()
                .AnyAsync(item => item.BuildingId == building_id, cancellationToken);
            if (hasFloorReference)
            {
                return BadRequest("Cannot delete building while floors still exist.");
            }

            var building = await context.Facilities
                .OfType<Building>()
                .FirstOrDefaultAsync(item => item.Id == building_id, cancellationToken);
            if (building != null)
            {
                context.Facilities.Remove(building);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{enterprise_role_id:guid}/facilities/floors")]
    [ProducesResponseType(typeof(FloorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEnterpriseFloor(
        [FromRoute] Guid enterprise_role_id,
        [FromBody] CreateFloorDto createDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var building = await context.Facilities
            .OfType<Building>()
            .FirstOrDefaultAsync(item =>
                    item.Id == createDto.BuildingId
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (building == null)
        {
            return NotFound();
        }

        var floorFacilityType = await context.FacilityTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityType.Floor, cancellationToken);
        if (floorFacilityType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityType '{FacilityType.Floor}' not found. Run database migration/seeding.");
        }

        var ownFacilityRoleType = await context.FacilityRoleTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityRoleType.Own, cancellationToken);
        if (ownFacilityRoleType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityRoleType '{FacilityRoleType.Own}' not found. Run database migration/seeding.");
        }

        var floor = new Floor
        {
            Id = Guid.NewGuid(),
            BuildingId = building.Id,
            PartOfId = building.Id,
            Level = createDto.Level,
            Description = string.IsNullOrWhiteSpace(createDto.Description) ? null : createDto.Description.Trim(),
            FacilityTypeId = floorFacilityType.Id
        };

        var facilityRole = new FacilityRole
        {
            Id = floor.Id,
            FacilityRoleTypeId = ownFacilityRoleType.Id,
            PartyId = enterprise.PartyId,
            PartyRoleTypeId = enterprise.TypeId
        };

        await context.AddAsync(floor, cancellationToken);
        await context.AddAsync(facilityRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Created($"/api/parties/enterprises/{enterprise_role_id}/facilities/floors/{floor.Id}", new FloorDto
        {
            FloorId = floor.Id!.Value,
            Level = floor.Level,
            Description = floor.Description,
            BuildingId = building.Id!.Value,
            BuildingName = building.Name ?? string.Empty,
            CreatedAtUtc = floor.CreatedAtUtc,
            UpdatedAtUtc = floor.UpdatedAtUtc,
            Revision = floor.Revision
        });
    }

    [HttpPut("{enterprise_role_id:guid}/facilities/floors/{floor_id:guid}")]
    [ProducesResponseType(typeof(FloorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseFloor(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid floor_id,
        [FromBody] UpdateFloorDto updateDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var floor = await context.Facilities
            .OfType<Floor>()
            .FirstOrDefaultAsync(item =>
                    item.Id == floor_id
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (floor == null)
        {
            return NotFound();
        }

        var building = await context.Facilities
            .OfType<Building>()
            .FirstOrDefaultAsync(item =>
                    item.Id == updateDto.BuildingId
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (building == null)
        {
            return NotFound();
        }

        floor.Level = updateDto.Level;
        floor.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
        floor.BuildingId = building.Id;
        floor.PartOfId = building.Id;

        await context.SaveChangesAsync(cancellationToken);

        return Ok(new FloorDto
        {
            FloorId = floor.Id!.Value,
            Level = floor.Level,
            Description = floor.Description,
            BuildingId = building.Id!.Value,
            BuildingName = building.Name ?? string.Empty,
            CreatedAtUtc = floor.CreatedAtUtc,
            UpdatedAtUtc = floor.UpdatedAtUtc,
            Revision = floor.Revision
        });
    }

    [HttpDelete("{enterprise_role_id:guid}/facilities/floors/{floor_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterpriseFloor(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid floor_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var enterpriseFloorRoles = await context.FacilityRoles
            .Where(role => role.Id == floor_id
                           && role.PartyId == enterprise.PartyId
                           && role.PartyRoleTypeId == enterprise.TypeId)
            .ToListAsync(cancellationToken);
        if (enterpriseFloorRoles.Count == 0)
        {
            return NotFound();
        }

        context.FacilityRoles.RemoveRange(enterpriseFloorRoles);

        var hasOtherFacilityRoleReferences = await context.FacilityRoles
            .AnyAsync(role => role.Id == floor_id
                              && (role.PartyId != enterprise.PartyId || role.PartyRoleTypeId != enterprise.TypeId),
                cancellationToken);
        if (!hasOtherFacilityRoleReferences)
        {
            var hasRoomReference = await context.Facilities
                .OfType<Room>()
                .AnyAsync(item => item.FloorId == floor_id, cancellationToken);
            if (hasRoomReference)
            {
                return BadRequest("Cannot delete floor while rooms still exist.");
            }

            var floor = await context.Facilities
                .OfType<Floor>()
                .FirstOrDefaultAsync(item => item.Id == floor_id, cancellationToken);
            if (floor != null)
            {
                context.Facilities.Remove(floor);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("{enterprise_role_id:guid}/facilities/rooms")]
    [ProducesResponseType(typeof(List<RoomDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnterpriseRooms(
        [FromRoute] Guid enterprise_role_id,
        [FromQuery] string? searchText,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var query = context.Facilities
            .OfType<Room>()
            .Include(room => room.Floor)
            .ThenInclude(floor => floor!.Building)
            .Where(room => context.FacilityRoles.Any(role =>
                role.Id == room.Id
                && role.PartyId == enterprise.PartyId
                && role.PartyRoleTypeId == enterprise.TypeId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var keyword = searchText.Trim();
            query = query.Where(room =>
                (room.Number != null && room.Number.Contains(keyword))
                || (room.Floor != null && room.Floor.Building != null && room.Floor.Building.Name != null && room.Floor.Building.Name.Contains(keyword))
                || room.Floor != null && room.Floor.Level.ToString().Contains(keyword));
        }

        var rooms = await query
            .OrderBy(room => room.Number)
            .Select(room => new RoomDto
            {
                RoomId = room.Id!.Value,
                Number = room.Number ?? string.Empty,
                Description = room.Description,
                FloorId = room.FloorId ?? Guid.Empty,
                FloorLevel = room.Floor != null ? room.Floor.Level : 0,
                BuildingId = room.Floor != null && room.Floor.BuildingId.HasValue ? room.Floor.BuildingId.Value : Guid.Empty,
                BuildingName = room.Floor != null && room.Floor.Building != null ? room.Floor.Building.Name ?? string.Empty : string.Empty,
                BedCount = context.Facilities
                    .OfType<Bed>()
                    .Count(bed =>
                        bed.RoomId == room.Id
                        && context.FacilityRoles.Any(role =>
                            role.Id == bed.Id
                            && role.PartyId == enterprise.PartyId
                            && role.PartyRoleTypeId == enterprise.TypeId)),
                CreatedAtUtc = room.CreatedAtUtc,
                UpdatedAtUtc = room.UpdatedAtUtc,
                Revision = room.Revision
            })
            .ToListAsync(cancellationToken);

        return Ok(rooms);
    }

    [HttpPost("{enterprise_role_id:guid}/facilities/rooms")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEnterpriseRoom(
        [FromRoute] Guid enterprise_role_id,
        [FromBody] CreateRoomDto createDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var floor = await context.Facilities
            .OfType<Floor>()
            .Include(item => item.Building)
            .FirstOrDefaultAsync(item =>
                    item.Id == createDto.FloorId
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (floor == null || !floor.BuildingId.HasValue)
        {
            return NotFound();
        }

        var roomFacilityType = await context.FacilityTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityType.Room, cancellationToken);
        if (roomFacilityType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityType '{FacilityType.Room}' not found. Run database migration/seeding.");
        }

        var ownFacilityRoleType = await context.FacilityRoleTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityRoleType.Own, cancellationToken);
        if (ownFacilityRoleType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityRoleType '{FacilityRoleType.Own}' not found. Run database migration/seeding.");
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Number = createDto.Number.Trim(),
            Description = string.IsNullOrWhiteSpace(createDto.Description) ? null : createDto.Description.Trim(),
            FloorId = floor.Id,
            PartOfId = floor.Id,
            FacilityTypeId = roomFacilityType.Id
        };

        var facilityRole = new FacilityRole
        {
            Id = room.Id,
            FacilityRoleTypeId = ownFacilityRoleType.Id,
            PartyId = enterprise.PartyId,
            PartyRoleTypeId = enterprise.TypeId
        };

        await context.AddAsync(room, cancellationToken);
        await context.AddAsync(facilityRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Room created: {RoomId}", room.Id);

        return Created($"/api/parties/enterprises/{enterprise_role_id}/facilities/rooms/{room.Id}", new RoomDto
        {
            RoomId = room.Id!.Value,
            Number = room.Number ?? string.Empty,
            Description = room.Description,
            FloorId = floor.Id!.Value,
            FloorLevel = floor.Level,
            BuildingId = floor.BuildingId.Value,
            BuildingName = floor.Building?.Name ?? string.Empty,
            BedCount = 0,
            CreatedAtUtc = room.CreatedAtUtc,
            UpdatedAtUtc = room.UpdatedAtUtc,
            Revision = room.Revision
        });
    }

    [HttpPut("{enterprise_role_id:guid}/facilities/rooms/{room_id:guid}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseRoom(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid room_id,
        [FromBody] UpdateRoomDto updateDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var room = await context.Facilities
            .OfType<Room>()
            .FirstOrDefaultAsync(item =>
                    item.Id == room_id
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (room == null)
        {
            return NotFound();
        }

        var floor = await context.Facilities
            .OfType<Floor>()
            .Include(item => item.Building)
            .FirstOrDefaultAsync(item =>
                    item.Id == updateDto.FloorId
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (floor == null || !floor.BuildingId.HasValue)
        {
            return NotFound();
        }

        room.Number = updateDto.Number.Trim();
        room.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
        room.FloorId = floor.Id;
        room.PartOfId = floor.Id;
        await context.SaveChangesAsync(cancellationToken);

        var roomBedCount = await context.Facilities
            .OfType<Bed>()
            .CountAsync(bed =>
                    bed.RoomId == room.Id
                    && context.FacilityRoles.Any(role =>
                        role.Id == bed.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);

        return Ok(new RoomDto
        {
            RoomId = room.Id!.Value,
            Number = room.Number ?? string.Empty,
            Description = room.Description,
            FloorId = floor.Id!.Value,
            FloorLevel = floor.Level,
            BuildingId = floor.BuildingId.Value,
            BuildingName = floor.Building?.Name ?? string.Empty,
            BedCount = roomBedCount,
            CreatedAtUtc = room.CreatedAtUtc,
            UpdatedAtUtc = room.UpdatedAtUtc,
            Revision = room.Revision
        });
    }

    [HttpDelete("{enterprise_role_id:guid}/facilities/rooms/{room_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterpriseRoom(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid room_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var enterpriseRoomRoles = await context.FacilityRoles
            .Where(role => role.Id == room_id
                           && role.PartyId == enterprise.PartyId
                           && role.PartyRoleTypeId == enterprise.TypeId)
            .ToListAsync(cancellationToken);
        if (enterpriseRoomRoles.Count == 0)
        {
            return NotFound();
        }

        context.FacilityRoles.RemoveRange(enterpriseRoomRoles);

        var hasOtherFacilityRoleReferences = await context.FacilityRoles
            .AnyAsync(role => role.Id == room_id
                              && (role.PartyId != enterprise.PartyId || role.PartyRoleTypeId != enterprise.TypeId),
                cancellationToken);
        if (!hasOtherFacilityRoleReferences)
        {
            var hasBedReference = await context.Facilities
                .OfType<Bed>()
                .AnyAsync(item => item.RoomId == room_id, cancellationToken);
            if (hasBedReference)
            {
                return BadRequest("Cannot delete room while beds still exist.");
            }

            var room = await context.Facilities
                .OfType<Room>()
                .FirstOrDefaultAsync(item => item.Id == room_id, cancellationToken);
            if (room != null)
            {
                context.Facilities.Remove(room);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("{enterprise_role_id:guid}/facilities/beds")]
    [ProducesResponseType(typeof(List<BedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnterpriseBeds(
        [FromRoute] Guid enterprise_role_id,
        [FromQuery] string? searchText,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var query = context.Facilities
            .OfType<Bed>()
            .Include(bed => bed.Room)
            .ThenInclude(room => room!.Floor)
            .ThenInclude(floor => floor!.Building)
            .Where(bed => context.FacilityRoles.Any(role =>
                role.Id == bed.Id
                && role.PartyId == enterprise.PartyId
                && role.PartyRoleTypeId == enterprise.TypeId));

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var keyword = searchText.Trim();
            query = query.Where(bed =>
                (bed.Number != null && bed.Number.Contains(keyword))
                || (bed.Room != null && bed.Room.Number != null && bed.Room.Number.Contains(keyword))
                || (bed.Room != null && bed.Room.Floor != null && bed.Room.Floor.Building != null && bed.Room.Floor.Building.Name != null && bed.Room.Floor.Building.Name.Contains(keyword)));
        }

        var beds = await query
            .OrderBy(bed => bed.Number)
            .Select(bed => new BedDto
            {
                BedId = bed.Id!.Value,
                Number = bed.Number ?? string.Empty,
                Description = bed.Description,
                RoomId = bed.RoomId!.Value,
                RoomNumber = bed.Room != null && bed.Room.Number != null ? bed.Room.Number : string.Empty,
                FloorId = bed.Room != null ? bed.Room.FloorId : null,
                FloorLevel = bed.Room != null && bed.Room.Floor != null ? bed.Room.Floor.Level : null,
                BuildingId = bed.Room != null && bed.Room.Floor != null ? bed.Room.Floor.BuildingId : null,
                BuildingName = bed.Room != null && bed.Room.Floor != null && bed.Room.Floor.Building != null ? bed.Room.Floor.Building.Name : null,
                CreatedAtUtc = bed.CreatedAtUtc,
                UpdatedAtUtc = bed.UpdatedAtUtc,
                Revision = bed.Revision
            })
            .ToListAsync(cancellationToken);

        return Ok(beds);
    }

    [HttpPost("{enterprise_role_id:guid}/facilities/beds")]
    [ProducesResponseType(typeof(BedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEnterpriseBed(
        [FromRoute] Guid enterprise_role_id,
        [FromBody] CreateBedDto createDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var room = await context.Facilities
            .OfType<Room>()
            .Include(item => item.Floor)
            .ThenInclude(item => item!.Building)
            .FirstOrDefaultAsync(item =>
                    item.Id == createDto.RoomId
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (room == null)
        {
            return NotFound();
        }

        var bedFacilityType = await context.FacilityTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityType.Bed, cancellationToken);
        if (bedFacilityType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityType '{FacilityType.Bed}' not found. Run database migration/seeding.");
        }

        var ownFacilityRoleType = await context.FacilityRoleTypes
            .SingleOrDefaultAsync(item => item.Code == FacilityRoleType.Own, cancellationToken);
        if (ownFacilityRoleType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"FacilityRoleType '{FacilityRoleType.Own}' not found. Run database migration/seeding.");
        }

        var bed = new Bed
        {
            Id = Guid.NewGuid(),
            Number = createDto.Number.Trim(),
            Description = string.IsNullOrWhiteSpace(createDto.Description) ? null : createDto.Description.Trim(),
            FacilityTypeId = bedFacilityType.Id,
            RoomId = room.Id,
            PartOfId = room.Id
        };

        var facilityRole = new FacilityRole
        {
            Id = bed.Id,
            FacilityRoleTypeId = ownFacilityRoleType.Id,
            PartyId = enterprise.PartyId,
            PartyRoleTypeId = enterprise.TypeId
        };

        await context.AddAsync(bed, cancellationToken);
        await context.AddAsync(facilityRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Created($"/api/parties/enterprises/{enterprise_role_id}/facilities/beds/{bed.Id}", new BedDto
        {
            BedId = bed.Id!.Value,
            Number = bed.Number ?? string.Empty,
            Description = bed.Description,
            RoomId = room.Id!.Value,
            RoomNumber = room.Number ?? string.Empty,
            FloorId = room.FloorId,
            FloorLevel = room.Floor?.Level,
            BuildingId = room.Floor?.BuildingId,
            BuildingName = room.Floor?.Building?.Name,
            CreatedAtUtc = bed.CreatedAtUtc,
            UpdatedAtUtc = bed.UpdatedAtUtc,
            Revision = bed.Revision
        });
    }

    [HttpPut("{enterprise_role_id:guid}/facilities/beds/{bed_id:guid}")]
    [ProducesResponseType(typeof(BedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnterpriseBed(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid bed_id,
        [FromBody] UpdateBedDto updateDto,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var bed = await context.Facilities
            .OfType<Bed>()
            .Include(item => item.Room)
            .ThenInclude(item => item!.Floor)
            .ThenInclude(item => item!.Building)
            .FirstOrDefaultAsync(item =>
                    item.Id == bed_id
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (bed == null)
        {
            return NotFound();
        }

        var room = await context.Facilities
            .OfType<Room>()
            .Include(item => item.Floor)
            .ThenInclude(item => item!.Building)
            .FirstOrDefaultAsync(item =>
                    item.Id == updateDto.RoomId
                    && context.FacilityRoles.Any(role =>
                        role.Id == item.Id
                        && role.PartyId == enterprise.PartyId
                        && role.PartyRoleTypeId == enterprise.TypeId),
                cancellationToken);
        if (room == null)
        {
            return NotFound();
        }

        bed.Number = updateDto.Number.Trim();
        bed.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
        bed.RoomId = room.Id;
        bed.PartOfId = room.Id;

        await context.SaveChangesAsync(cancellationToken);

        return Ok(new BedDto
        {
            BedId = bed.Id!.Value,
            Number = bed.Number ?? string.Empty,
            Description = bed.Description,
            RoomId = room.Id!.Value,
            RoomNumber = room.Number ?? string.Empty,
            FloorId = room.FloorId,
            FloorLevel = room.Floor?.Level,
            BuildingId = room.Floor?.BuildingId,
            BuildingName = room.Floor?.Building?.Name,
            CreatedAtUtc = bed.CreatedAtUtc,
            UpdatedAtUtc = bed.UpdatedAtUtc,
            Revision = bed.Revision
        });
    }

    [HttpDelete("{enterprise_role_id:guid}/facilities/beds/{bed_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterpriseBed(
        [FromRoute] Guid enterprise_role_id,
        [FromRoute] Guid bed_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var enterprise = await context.PartyRoles
            .OfType<Enterprise>()
            .Select(e => new { e.Id, e.PartyId, e.TypeId })
            .FirstOrDefaultAsync(e => e.Id == enterprise_role_id, cancellationToken);
        if (enterprise == null || !enterprise.PartyId.HasValue || !enterprise.TypeId.HasValue)
        {
            return NotFound();
        }

        var enterpriseBedRoles = await context.FacilityRoles
            .Where(role => role.Id == bed_id
                           && role.PartyId == enterprise.PartyId
                           && role.PartyRoleTypeId == enterprise.TypeId)
            .ToListAsync(cancellationToken);
        if (enterpriseBedRoles.Count == 0)
        {
            return NotFound();
        }

        context.FacilityRoles.RemoveRange(enterpriseBedRoles);

        var hasOtherFacilityRoleReferences = await context.FacilityRoles
            .AnyAsync(role => role.Id == bed_id
                              && (role.PartyId != enterprise.PartyId || role.PartyRoleTypeId != enterprise.TypeId),
                cancellationToken);
        if (!hasOtherFacilityRoleReferences)
        {
            var bed = await context.Facilities
                .OfType<Bed>()
                .FirstOrDefaultAsync(item => item.Id == bed_id, cancellationToken);
            if (bed != null)
            {
                context.Facilities.Remove(bed);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("legal-structures")]
    [ProducesResponseType(typeof(List<LegalStructure>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLegalStructures(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();
        var legalStructures = await context.LegalStructures.ToListAsync(cancellationToken);
        return Ok(legalStructures);
    }

    [HttpPost("legal-structures")]
    [ProducesResponseType(typeof(LegalStructure), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLegalStructure(
        [FromBody] CreateLegalStructure createLegalStructure,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var legalStructure = new LegalStructure
        {
            Id = Guid.NewGuid(),
            Name = createLegalStructure.Name,
            Code = createLegalStructure.Code
        };

        await context.AddAsync(legalStructure, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("LegalStructure created: {LegalStructureId}", legalStructure.Id);

        return CreatedAtAction(
            nameof(GetLegalStructure),
            new { legal_structure_id = legalStructure.Id },
            legalStructure);
    }

    [HttpGet("legal-structures/{legal_structure_id:guid}")]
    [ProducesResponseType(typeof(LegalStructure), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLegalStructure(
        [FromRoute] Guid legal_structure_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var legalStructure = await context.LegalStructures.FindAsync(legal_structure_id, cancellationToken);
        if (legalStructure == null)
        {
            return NotFound();
        }

        return Ok(legalStructure);
    }

    [HttpPatch("legal-structures/{legal_structure_id:guid}")]
    [ProducesResponseType(typeof(LegalStructure), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchLegalStructure(
        [FromRoute] Guid legal_structure_id,
        [FromBody] JsonPatchDocument<LegalStructure> patchDoc,
        CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var legalStructure = await context.LegalStructures.FindAsync(legal_structure_id, cancellationToken);
        if (legalStructure == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(legalStructure);
        await context.SaveChangesAsync(cancellationToken);

        return Ok(legalStructure);
    }

    [HttpDelete("legal-structures/{legal_structure_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLegalStructure(
        [FromRoute] Guid legal_structure_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var legalStructure = await context.LegalStructures.FindAsync(legal_structure_id, cancellationToken);
        if (legalStructure == null)
        {
            return NotFound();
        }

        context.LegalStructures.Remove(legalStructure);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
