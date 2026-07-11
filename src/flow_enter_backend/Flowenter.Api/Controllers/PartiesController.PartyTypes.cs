using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class PartiesController
{
    [HttpGet("types")]
    [ProducesResponseType(typeof(List<PartyType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartyTypes(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyTypes = await context.PartyTypes.ToListAsync(cancellationToken);

        return Ok(partyTypes);
    }

    [HttpPost("types")]
    [ProducesResponseType(typeof(PartyType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartyType(CreatePartyType createPartyType
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();
        // Map DTO to entity
        var partyType = new PartyType
        {
            Id = Guid.NewGuid(),
            Name = createPartyType.Name,
            Code = createPartyType.Code
        };

        await context.AddAsync(partyType);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PartyType created: {PartyTypeId}", partyType.Id);

        return CreatedAtAction(
            nameof(GetPartyType),
            new { party_type_id = partyType.Id },
            partyType);
    }

    [HttpGet("types/{party_type_id}")]
    [ProducesResponseType(typeof(PartyType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartyType(Guid party_type_id, CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyType = await context.PartyTypes.FindAsync(party_type_id, cancellationToken);
        if (partyType == null)
        {
            return NotFound();
        }

        return Ok(partyType);
    }

    [HttpPatch("types/{party_type_id}")]
    [ProducesResponseType(typeof(PartyType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchPartyType(Guid party_type_id
        , [FromBody] JsonPatchDocument<PartyType> patchDoc
        , CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var partyType = await context.PartyTypes.FindAsync(party_type_id, cancellationToken);
        if (partyType == null)
        {
            return NotFound();
        }

        // Apply the patch document to the existing product object
        patchDoc.ApplyTo(partyType);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(partyType);
    }

    [HttpDelete("types/{party_type_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartyType(Guid party_type_id, CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyType = await context.PartyTypes.FindAsync(party_type_id, cancellationToken);
        if (partyType == null)
        {
            return NotFound();
        }

        context.PartyTypes.Remove(partyType);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
