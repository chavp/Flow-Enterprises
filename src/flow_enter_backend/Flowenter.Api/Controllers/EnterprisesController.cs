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
            .Where(item => item.EmployerId == enterpriseRoleId || item.EmployeeId == enterpriseRoleId)
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
            where employment.EmployerId == enterprise_role_id
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

                return new EmploymentDto
                {
                    EmploymentId = row.Employment.Id!.Value,
                    EmployerId = row.Employment.EmployerId!.Value,
                    EmployeePartyRoleId = row.EmployeeRole.Id!.Value,
                    EmployeePartyId = row.PersonId!.Value,
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
        var firstEmployeeRole = default(Customer);
        var firstEmployment = default(Employment);

        await context.AddRangeAsync(person, personName);

        foreach (var partyRoleType in partyRoleTypes)
        {
            var employeeRole = new Customer
            {
                Id = Guid.NewGuid(),
                TypeId = partyRoleType.Id,
                PartyId = person.Id
            };

            var employment = new Employment(enterpriseRole.Id!.Value, employeeRole.Id!.Value, employmentRelationshipType.Id!.Value)
            {
                Id = Guid.NewGuid(),
                PartyRelationshipTypeId = employmentRelationshipType.Id,
                EmployerId = enterpriseRole.Id,
                EmployeeId = employeeRole.Id
            };

            await context.AddRangeAsync(employeeRole, employment);

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

        var response = new EmploymentDto
        {
            EmploymentId = firstEmployment!.Id!.Value,
            EmployerId = firstEmployment.EmployerId!.Value,
            EmployeePartyRoleId = firstEmployeeRole!.Id!.Value,
            EmployeePartyId = person.Id!.Value,
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
            .FirstOrDefaultAsync(item => item.Id == employment_id && item.EmployerId == enterprise_role_id, cancellationToken);
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
            where employmentItem.EmployerId == enterprise_role_id
                  && role.PartyId == person.Id
            select new
            {
                Employment = employmentItem,
                EmployeeRole = role
            })
            .ToListAsync(cancellationToken);

        var existingByRoleTypeId = relatedEmployments
            .Where(item => item.EmployeeRole.TypeId.HasValue)
            .GroupBy(item => item.EmployeeRole.TypeId!.Value)
            .ToDictionary(group => group.Key, group => group.First());

        var selectedRoleTypeSet = selectedRoleTypeIds.ToHashSet();
        var selectedPartyRoleTypeMap = selectedPartyRoleTypes
            .Where(item => item.Id.HasValue)
            .ToDictionary(item => item.Id!.Value, item => item);

        foreach (var relatedEmployment in relatedEmployments)
        {
            var typeId = relatedEmployment.EmployeeRole.TypeId;
            if (!typeId.HasValue || selectedRoleTypeSet.Contains(typeId.Value))
            {
                continue;
            }

            context.PartyRelationships.Remove(relatedEmployment.Employment);
            context.PartyRoles.Remove(relatedEmployment.EmployeeRole);
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

        var createdEmployments = new Dictionary<Guid, (Employment Employment, Customer EmployeeRole)>();
        foreach (var selectedRoleTypeId in selectedRoleTypeIds)
        {
            if (existingByRoleTypeId.ContainsKey(selectedRoleTypeId))
            {
                continue;
            }

            var employeeRoleToAdd = new Customer
            {
                Id = Guid.NewGuid(),
                TypeId = selectedRoleTypeId,
                PartyId = person.Id
            };

            var employmentToAdd = new Employment(enterprise_role_id, employeeRoleToAdd.Id!.Value, employmentRelationshipType.Id!.Value)
            {
                Id = Guid.NewGuid(),
                PartyRelationshipTypeId = employmentRelationshipType.Id,
                EmployerId = enterprise_role_id,
                EmployeeId = employeeRoleToAdd.Id
            };

            await context.AddRangeAsync(employeeRoleToAdd, employmentToAdd);
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

        return Ok(new EmploymentDto
        {
            EmploymentId = responseEmployment.Id!.Value,
            EmployerId = responseEmployment.EmployerId!.Value,
            EmployeePartyRoleId = responseEmployeeRoleId!.Value,
            EmployeePartyId = person.Id!.Value,
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
            where employment.Id == employment_id && employment.EmployerId == enterprise_role_id
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

        return Ok(new EmploymentDto
        {
            EmploymentId = employmentRow.Employment.Id!.Value,
            EmployerId = employmentRow.Employment.EmployerId!.Value,
            EmployeePartyRoleId = employmentRow.EmployeeRole.Id!.Value,
            EmployeePartyId = employmentRow.EmployeeRole.PartyId!.Value,
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
            .FirstOrDefaultAsync(item => item.Id == employment_id && item.EmployerId == enterprise_role_id, cancellationToken);
        if (employment == null)
        {
            return NotFound();
        }

        var employeeRole = await context.PartyRoles
            .FirstOrDefaultAsync(item => item.Id == employment.EmployeeId, cancellationToken);
        if (employeeRole?.PartyId == null)
        {
            context.PartyRelationships.Remove(employment);
            await context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        var relatedEmployments = await (
            from employmentItem in context.PartyRelationships.OfType<Employment>()
            join role in context.PartyRoles on employmentItem.EmployeeId equals role.Id
            where employmentItem.EmployerId == enterprise_role_id
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
            .Where(room => context.FacilityRoles.Any(role =>
                role.Id == room.Id
                && role.PartyId == enterprise.PartyId
                && role.PartyRoleTypeId == enterprise.TypeId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var keyword = searchText.Trim();
            query = query.Where(room => room.Number != null && room.Number.Contains(keyword));
        }

        var rooms = await query
            .OrderBy(room => room.Number)
            .Select(room => new RoomDto
            {
                RoomId = room.Id!.Value,
                Number = room.Number ?? string.Empty,
                Description = room.Description,
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

        room.Number = updateDto.Number.Trim();
        room.Description = string.IsNullOrWhiteSpace(updateDto.Description) ? null : updateDto.Description.Trim();
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
            BedCount = roomBedCount,
            CreatedAtUtc = room.CreatedAtUtc,
            UpdatedAtUtc = room.UpdatedAtUtc,
            Revision = room.Revision
        });
    }

    [HttpDelete("{enterprise_role_id:guid}/facilities/rooms/{room_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
            .Where(bed => context.FacilityRoles.Any(role =>
                role.Id == bed.Id
                && role.PartyId == enterprise.PartyId
                && role.PartyRoleTypeId == enterprise.TypeId));

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var keyword = searchText.Trim();
            query = query.Where(bed =>
                (bed.Number != null && bed.Number.Contains(keyword))
                || (bed.Room != null && bed.Room.Number != null && bed.Room.Number.Contains(keyword)));
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
            RoomId = room.Id
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

        await context.SaveChangesAsync(cancellationToken);

        return Ok(new BedDto
        {
            BedId = bed.Id!.Value,
            Number = bed.Number ?? string.Empty,
            Description = bed.Description,
            RoomId = room.Id!.Value,
            RoomNumber = room.Number ?? string.Empty,
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
