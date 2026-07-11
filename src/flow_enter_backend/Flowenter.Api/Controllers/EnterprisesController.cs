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
