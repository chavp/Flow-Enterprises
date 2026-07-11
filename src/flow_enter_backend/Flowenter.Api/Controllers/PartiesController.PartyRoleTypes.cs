using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class PartiesController
{
    [HttpGet("role-types")]
    [ProducesResponseType(typeof(List<PartyRoleType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartyRoleTypes(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyRoleTypes = await context.PartyRoleTypes.ToListAsync(cancellationToken);

        return Ok(partyRoleTypes);
    }

    [HttpPost("role-types")]
    [ProducesResponseType(typeof(PartyRoleType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartyRoleType(CreatePartyRoleType createPartyRoleType
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyRoleType = new PartyRoleType
        {
            Id = Guid.NewGuid(),
            Name = createPartyRoleType.Name,
            Code = createPartyRoleType.Code
        };

        await context.AddAsync(partyRoleType);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PartyRoleType created: {PartyRoleTypeId}", partyRoleType.Id);

        return CreatedAtAction(
            nameof(GetPartyRoleType),
            new { party_role_type_id = partyRoleType.Id },
            partyRoleType);
    }

    [HttpGet("role-types/{party_role_type_id}")]
    [ProducesResponseType(typeof(PartyRoleType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartyRoleType(Guid party_role_type_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyRoleType = await context.PartyRoleTypes
            .FindAsync(party_role_type_id, cancellationToken);
        if (partyRoleType == null)
        {
            return NotFound();
        }

        return Ok(partyRoleType);
    }

    [HttpPatch("role-types/{party_role_type_id}")]
    [ProducesResponseType(typeof(PartyRoleType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchPartyRoleType(Guid party_role_type_id
        , [FromBody] JsonPatchDocument<PartyRoleType> patchDoc
        , CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var partyRoleType = await context.PartyRoleTypes
            .FindAsync(party_role_type_id, cancellationToken);
        if (partyRoleType == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(partyRoleType);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(partyRoleType);
    }

    [HttpDelete("role-types/{party_role_type_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartyRoleType(Guid party_role_type_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyRoleType = await context.PartyRoleTypes.FindAsync(party_role_type_id, cancellationToken);
        if (partyRoleType == null)
        {
            return NotFound();
        }

        context.PartyRoleTypes.Remove(partyRoleType);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
