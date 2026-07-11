using Flowenter.Api.Extensions;
using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.IServices.Dtos.EnterpriseDto;
using Flowenter.Parties.Mappings;
using Flowenter.Parties.Mappings.Extensions;
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
                                 && personName.FromDate <= today
                                 && today <= personName.ThruDate)
            .OrderByDescending(personName => personName.FromDate)
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
                    EmployeeFullName = fullName,
                    PartyRoleTypeId = row.RoleType.Id!.Value,
                    PartyRoleTypeCode = row.RoleType.Code ?? string.Empty,
                    PartyRoleTypeName = row.RoleType.Name ?? string.Empty,
                    FromDate = row.Employment.FromDate,
                    ThruDate = row.Employment.ThruDate,
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

        var partyRoleType = await context.PartyRoleTypes
            .SingleOrDefaultAsync(type => type.Id == createDto.PartyRoleTypeId, cancellationToken);
        if (partyRoleType == null)
        {
            return BadRequest($"PartyRoleType '{createDto.PartyRoleTypeId}' not found.");
        }

        var language = createDto.LanguageId.HasValue
            ? await context.Languages.FirstOrDefaultAsync(item => item.Id == createDto.LanguageId, cancellationToken)
            : await context.Languages.FirstOrDefaultAsync(item => item.Code == Language.TH, cancellationToken);

        if (language == null)
        {
            language = await context.Languages.FirstOrDefaultAsync(cancellationToken);
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

        var employeeRole = new Customer
        {
            Id = Guid.NewGuid(),
            TypeId = partyRoleType.Id,
            PartyId = person.Id
        };

        var employment = new Employment(enterpriseRole.Id!.Value, employeeRole.Id!.Value, employmentRelationshipType)
        {
            Id = Guid.NewGuid(),
            PartyRelationshipTypeId = employmentRelationshipType.Id,
            EmployerId = enterpriseRole.Id,
            EmployeeId = employeeRole.Id
        };

        await context.AddRangeAsync(person, personName, employeeRole, employment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await tran.CommitAsync(cancellationToken);

        _logger.LogInformation("Employment created: {EmploymentId} for enterprise {EnterpriseRoleId}", employment.Id, enterprise_role_id);

        var fullName = string.Join(" ",
            new[] { personName.FirstName, personName.MiddleName, personName.LastName }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

        var response = new EmploymentDto
        {
            EmploymentId = employment.Id!.Value,
            EmployerId = employment.EmployerId!.Value,
            EmployeePartyRoleId = employeeRole.Id!.Value,
            EmployeePartyId = person.Id!.Value,
            EmployeeFullName = fullName,
            PartyRoleTypeId = partyRoleType.Id!.Value,
            PartyRoleTypeCode = partyRoleType.Code ?? string.Empty,
            PartyRoleTypeName = partyRoleType.Name ?? string.Empty,
            FromDate = employment.FromDate,
            ThruDate = employment.ThruDate,
            CreatedAtUtc = employment.CreatedAtUtc,
            UpdatedAtUtc = employment.UpdatedAtUtc,
            Revision = employment.Revision
        };

        return Created($"/api/parties/enterprises/{enterprise_role_id}/employments/{employment.Id}", response);
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
