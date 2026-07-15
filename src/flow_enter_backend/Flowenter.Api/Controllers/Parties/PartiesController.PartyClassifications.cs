using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers.Parties;

public partial class PartiesController
{
    [HttpGet("classifications")]
    [ProducesResponseType(typeof(List<PartyClassification>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartyClassifications(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyClassifications = await context.PartyClassifications
            .Include(pc => pc.Party)
            .Include(pc => pc.Category)
            .ToListAsync(cancellationToken);

        return Ok(partyClassifications);
    }

    [HttpPost("classifications")]
    [ProducesResponseType(typeof(PartyClassification), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartyClassification(CreatePartyClassification createPartyClassification
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyClassification = new PartyClassification
        {
            Id = Guid.NewGuid(),
            PartyId = createPartyClassification.PartyId,
            CategoryId = createPartyClassification.CategoryId
        };

        await context.AddAsync(partyClassification);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PartyClassification created: {PartyClassificationId}", partyClassification.Id);

        return CreatedAtAction(
            nameof(GetPartyClassification),
            new { party_classification_id = partyClassification.Id },
            partyClassification);
    }

    [HttpGet("classifications/{party_classification_id}")]
    [ProducesResponseType(typeof(PartyClassification), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartyClassification(Guid party_classification_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyClassification = await context.PartyClassifications
            .Include(pc => pc.Party)
            .Include(pc => pc.Category)
            .FirstOrDefaultAsync(pc => pc.Id == party_classification_id, cancellationToken);

        if (partyClassification == null)
        {
            return NotFound();
        }

        return Ok(partyClassification);
    }

    [HttpPatch("classifications/{party_classification_id}")]
    [ProducesResponseType(typeof(PartyClassification), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchPartyClassification(Guid party_classification_id
        , [FromBody] JsonPatchDocument<PartyClassification> patchDoc
        , CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var partyClassification = await context.PartyClassifications
            .FindAsync(party_classification_id, cancellationToken);
        if (partyClassification == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(partyClassification);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(partyClassification);
    }

    [HttpDelete("classifications/{party_classification_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartyClassification(Guid party_classification_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyClassification = await context.PartyClassifications
            .FindAsync(party_classification_id, cancellationToken);
        if (partyClassification == null)
        {
            return NotFound();
        }

        context.PartyClassifications.Remove(partyClassification);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
