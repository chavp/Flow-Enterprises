using Flowenter.Api.DTOs;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class PartiesController
{
    [HttpGet("party-types")]
    [ProducesResponseType(typeof(List<PartyType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartyTypes()
    {
        using var context = _factory.CreateDbContext();

        var partyTypes = await context.PartyTypes.ToListAsync();

        return Ok(partyTypes);
    }

    [HttpPost("party-types")]
    [ProducesResponseType(typeof(PartyType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartyType(CreatePartyType createPartyType)
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
        await context.SaveChangesAsync();

        _logger.LogInformation("PartyType created: {PartyTypeId}", partyType.Id);

        return CreatedAtAction(
            nameof(GetPartyType),
            new { partyTypeId = partyType.Id },
            partyType);
    }

    [HttpGet("party-types/{partyTypeId}")]
    [ProducesResponseType(typeof(PartyType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartyType([FromRoute] Guid partyTypeId)
    {
        using var context = _factory.CreateDbContext();

        var partyType = await context.PartyTypes.FindAsync(partyTypeId);
        if (partyType == null)
        {
            return NotFound();
        }

        return Ok(partyType);
    }

    [HttpPatch("party-types/{partyTypeId}")]
    [ProducesResponseType(typeof(PartyType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchPartyType([FromRoute] Guid partyTypeId,
            [FromBody] JsonPatchDocument<PartyType> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var partyType = await context.PartyTypes.FindAsync(partyTypeId);
        if (partyType == null)
        {
            return NotFound();
        }

        // Apply the patch document to the existing product object
        patchDoc.ApplyTo(partyType);

        await context.SaveChangesAsync();

        return Ok(partyType);
    }

    [HttpDelete("party-types/{partyTypeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartyType([FromRoute] Guid partyTypeId)
    {
        using var context = _factory.CreateDbContext();

        var partyType = await context.PartyTypes.FindAsync(partyTypeId);
        if (partyType == null)
        {
            return NotFound();
        }

        context.PartyTypes.Remove(partyType);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
