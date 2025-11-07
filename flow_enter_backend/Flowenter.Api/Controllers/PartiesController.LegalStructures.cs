using Flowenter.Parties.IServices.DTOs;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class PartiesController
{
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
    public async Task<IActionResult> CreateLegalStructure(CreateLegalStructure createLegalStructure
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var legalStructure = new LegalStructure
        {
            Id = Guid.NewGuid(),
            Name = createLegalStructure.Name,
            Code = createLegalStructure.Code
        };

        await context.AddAsync(legalStructure);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("LegalStructure created: {LegalStructureId}", legalStructure.Id);

        return CreatedAtAction(
            nameof(GetLegalStructure),
            new { legal_structure_id = legalStructure.Id },
            legalStructure);
    }

    [HttpGet("legal-structures/{legal_structure_id}")]
    [ProducesResponseType(typeof(LegalStructure), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLegalStructure(Guid legal_structure_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var legalStructure = await context.LegalStructures.FindAsync(legal_structure_id, cancellationToken);
        if (legalStructure == null)
        {
            return NotFound();
        }

        return Ok(legalStructure);
    }

    [HttpPatch("legal-structures/{legal_structure_id}")]
    [ProducesResponseType(typeof(LegalStructure), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchLegalStructure(Guid legal_structure_id
        , [FromBody] JsonPatchDocument<LegalStructure> patchDoc
        , CancellationToken cancellationToken)
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

    [HttpDelete("legal-structures/{legal_structure_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLegalStructure(Guid legal_structure_id
        , CancellationToken cancellationToken)
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
