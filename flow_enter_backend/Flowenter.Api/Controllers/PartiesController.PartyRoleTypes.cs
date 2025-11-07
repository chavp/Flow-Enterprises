using Flowenter.Api.DTOs;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class PartiesController
{
    [HttpGet("party-role-types")]
    [ProducesResponseType(typeof(List<PartyRoleType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartyRoleTypes()
    {
        using var context = _factory.CreateDbContext();

        var partyRoleTypes = await context.PartyRoleTypes.ToListAsync();

        return Ok(partyRoleTypes);
    }

    [HttpPost("party-role-types")]
    [ProducesResponseType(typeof(PartyRoleType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartyRoleType(CreatePartyRoleType createPartyRoleType)
    {
        using var context = _factory.CreateDbContext();

        var partyRoleType = new PartyRoleType
        {
            Id = Guid.NewGuid(),
            Name = createPartyRoleType.Name,
            Code = createPartyRoleType.Code
        };

        await context.AddAsync(partyRoleType);
        await context.SaveChangesAsync();

        _logger.LogInformation("PartyRoleType created: {PartyRoleTypeId}", partyRoleType.Id);

        return CreatedAtAction(
            nameof(GetPartyRoleType),
            new { partyRoleTypeId = partyRoleType.Id },
            partyRoleType);
    }

    [HttpGet("party-role-types/{partyRoleTypeId}")]
    [ProducesResponseType(typeof(PartyRoleType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartyRoleType([FromRoute] Guid partyRoleTypeId)
    {
        using var context = _factory.CreateDbContext();

        var partyRoleType = await context.PartyRoleTypes.FindAsync(partyRoleTypeId);
        if (partyRoleType == null)
        {
            return NotFound();
        }

        return Ok(partyRoleType);
    }

    [HttpPatch("party-role-types/{partyRoleTypeId}")]
    [ProducesResponseType(typeof(PartyRoleType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchPartyRoleType([FromRoute] Guid partyRoleTypeId,
            [FromBody] JsonPatchDocument<PartyRoleType> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var partyRoleType = await context.PartyRoleTypes.FindAsync(partyRoleTypeId);
        if (partyRoleType == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(partyRoleType);

        await context.SaveChangesAsync();

        return Ok(partyRoleType);
    }

    [HttpDelete("party-role-types/{partyRoleTypeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartyRoleType([FromRoute] Guid partyRoleTypeId)
    {
        using var context = _factory.CreateDbContext();

        var partyRoleType = await context.PartyRoleTypes.FindAsync(partyRoleTypeId);
        if (partyRoleType == null)
        {
            return NotFound();
        }

        context.PartyRoleTypes.Remove(partyRoleType);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
