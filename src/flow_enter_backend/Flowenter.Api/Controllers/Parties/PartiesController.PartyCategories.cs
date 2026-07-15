using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers.Parties;

public partial class PartiesController
{
    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<PartyCategory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPartyCategories(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyCategories = await context.PartyCategories
            .Include(pc => pc.GroupBy)
            .ToListAsync(cancellationToken);

        return Ok(partyCategories);
    }

    [HttpPost("categories")]
    [ProducesResponseType(typeof(PartyCategory), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartyCategory(CreatePartyCategory createPartyCategory
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyCategory = new PartyCategory
        {
            Id = Guid.NewGuid(),
            Name = createPartyCategory.Name,
            GroupById = createPartyCategory.GroupById
        };

        await context.AddAsync(partyCategory);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PartyCategory created: {PartyCategoryId}", partyCategory.Id);

        return CreatedAtAction(
            nameof(GetPartyCategory),
            new { party_category_id = partyCategory.Id },
            partyCategory);
    }

    [HttpGet("categories/{party_category_id}")]
    [ProducesResponseType(typeof(PartyCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPartyCategory(Guid party_category_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyCategory = await context.PartyCategories
            .Include(pc => pc.GroupBy)
            .SingleOrDefaultAsync(x => x.Id == party_category_id, cancellationToken);

        if (partyCategory == null)
        {
            return NotFound();
        }

        return Ok(partyCategory);
    }

    [HttpPatch("categories/{party_category_id}")]
    [ProducesResponseType(typeof(PartyCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchPartyCategory(Guid party_category_id
        , [FromBody] JsonPatchDocument<PartyCategory> patchDoc
        , CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var partyCategory = await context.PartyCategories
            .FindAsync(party_category_id, cancellationToken);
        if (partyCategory == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(partyCategory);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(partyCategory);
    }

    [HttpDelete("categories/{party_category_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePartyCategory(Guid party_category_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var partyCategory = await context
            .PartyCategories
            .FindAsync(party_category_id, cancellationToken);
        if (partyCategory == null)
        {
            return NotFound();
        }

        context.PartyCategories.Remove(partyCategory);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
