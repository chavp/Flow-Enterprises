using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.GeographicBoundaryModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class GeographicBoundariesController
{
    [HttpGet("countries")]
    [ProducesResponseType(typeof(List<Country>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountries(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var countries = await context.Countries
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            Data = countries,
            TotalCount = await context.Countries.CountAsync(cancellationToken),
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    [HttpPost("countries")]
    [ProducesResponseType(typeof(Country), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCountry(
        [FromBody] CreateCountry createCountry,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var boundaryType = await context.GeographicBoundaryTypes
            .FirstOrDefaultAsync(t => t.Code == GeographicBoundaryType.Country, cancellationToken);
        if (boundaryType == null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Missing master data",
                detail: $"GeographicBoundaryType '{GeographicBoundaryType.Country}' not found. Run database migration/seeding.");
        }

        var country = new Country
        {
            Id = Guid.NewGuid(),
            Type = boundaryType,
            Name = createCountry.Name,
            Nationality = createCountry.Nationality ?? string.Empty,
            Numeric = createCountry.Numeric,
            IsoCode2 = createCountry.IsoCode2,
            IsoCode3 = createCountry.IsoCode3
        };

        await context.AddAsync(country, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Country created: {CountryId}", country.Id);

        return CreatedAtAction(nameof(GetCountry), new { country_id = country.Id }, country);
    }

    [HttpGet("countries/{country_id:guid}")]
    [ProducesResponseType(typeof(Country), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCountry([FromRoute] Guid country_id, CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var country = await context.Countries.FindAsync(country_id, cancellationToken);
        if (country == null)
        {
            return NotFound();
        }

        return Ok(country);
    }

    [HttpPatch("countries/{country_id:guid}")]
    [ProducesResponseType(typeof(Country), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchCountry(
        [FromRoute] Guid country_id,
        [FromBody] JsonPatchDocument<Country> patchDoc,
        CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var country = await context.Countries.FindAsync(country_id, cancellationToken);
        if (country == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(country, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Ok(country);
    }

    [HttpDelete("countries/{country_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCountry([FromRoute] Guid country_id, CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var country = await context.Countries.FindAsync(country_id, cancellationToken);
        if (country == null)
        {
            return NotFound();
        }

        context.Countries.Remove(country);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
