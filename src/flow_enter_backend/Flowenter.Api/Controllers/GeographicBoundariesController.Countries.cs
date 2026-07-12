using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.GeographicBoundaryModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class GeographicBoundariesController
{
    [HttpGet("countries/tree")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountriesTree(CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var countries = await context.GeographicBoundaries
            .OfType<Country>()
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        var countryIds = countries
            .Where(item => item.Id.HasValue)
            .Select(item => item.Id!.Value)
            .ToList();

        var provinces = await context.GeographicBoundaries
            .OfType<Province>()
            .Where(item => item.CountryId.HasValue && countryIds.Contains(item.CountryId.Value))
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        var provincesByCountryId = provinces
            .Where(item => item.CountryId.HasValue)
            .GroupBy(item => item.CountryId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        var result = countries.Select(country =>
        {
            var countryId = country.Id!.Value;
            var countryProvinces = provincesByCountryId.TryGetValue(countryId, out var values)
                ? values
                : [];

            return new
            {
                Country = country,
                Provinces = countryProvinces
            };
        });

        return Ok(result);
    }

    [HttpGet("countries")]
    [ProducesResponseType(typeof(List<Country>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountries(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var countries = await context
            .GeographicBoundaries
            .OfType<Country>()
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            Data = countries,
            TotalCount = await context
            .GeographicBoundaries
            .OfType<Country>()
            .CountAsync(cancellationToken),
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

        var country = await context
            .GeographicBoundaries
            .OfType<Country>()
            .SingleOrDefaultAsync(x => x.Id == country_id, cancellationToken);
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

        var country = await context
            .GeographicBoundaries
            .OfType<Country>()
            .SingleAsync( c => c.Id == country_id, cancellationToken);
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

        var country = await context.GeographicBoundaries
            .OfType<Country>()
            .SingleOrDefaultAsync(x => x.Id == country_id, cancellationToken);
        if (country == null)
        {
            return NotFound();
        }

        var provinces = await context.GeographicBoundaries
            .OfType<Province>()
            .Where(item => item.CountryId == country_id)
            .ToListAsync(cancellationToken);
        if (provinces.Count > 0)
        {
            context.RemoveRange(provinces);
        }

        context.Remove(country);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("countries/{country_id:guid}/provinces")]
    [ProducesResponseType(typeof(List<Province>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCountryProvinces(
        [FromRoute] Guid country_id,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var countryExists = await context.GeographicBoundaries
            .OfType<Country>()
            .AnyAsync(item => item.Id == country_id, cancellationToken);
        if (!countryExists)
        {
            return NotFound();
        }

        var provinces = await context.GeographicBoundaries
            .OfType<Province>()
            .Where(item => item.CountryId == country_id)
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        return Ok(provinces);
    }

    [HttpPost("countries/{country_id:guid}/provinces")]
    [ProducesResponseType(typeof(Province), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateProvince(
        [FromRoute] Guid country_id,
        [FromBody] CreateProvince createProvince,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var country = await context.GeographicBoundaries
            .OfType<Country>()
            .FirstOrDefaultAsync(item => item.Id == country_id, cancellationToken);
        if (country == null)
        {
            return NotFound();
        }

        var provinceBoundaryType = await context.GeographicBoundaryTypes
            .FirstOrDefaultAsync(item => item.Code == GeographicBoundaryType.Province, cancellationToken);
        if (provinceBoundaryType == null)
        {
            provinceBoundaryType = new GeographicBoundaryType
            {
                Id = Guid.NewGuid(),
                Code = GeographicBoundaryType.Province,
                Name = "Province"
            };
            await context.AddAsync(provinceBoundaryType, cancellationToken);
        }

        var province = new Province
        {
            Id = Guid.NewGuid(),
            Type = provinceBoundaryType,
            CountryId = country.Id,
            Name = createProvince.Name?.Trim(),
            Hs = createProvince.Hs?.Trim(),
            Iso = createProvince.Iso?.Trim(),
            Fips = createProvince.Fips?.Trim()
        };

        await context.AddAsync(province, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Created($"/api/geographic-boundaries/provinces/{province.Id}", province);
    }

    [HttpPatch("provinces/{province_id:guid}")]
    [ProducesResponseType(typeof(Province), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchProvince(
        [FromRoute] Guid province_id,
        [FromBody] JsonPatchDocument<Province> patchDoc,
        CancellationToken cancellationToken = default)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var province = await context.GeographicBoundaries
            .OfType<Province>()
            .SingleOrDefaultAsync(item => item.Id == province_id, cancellationToken);
        if (province == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(province, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Ok(province);
    }

    [HttpDelete("provinces/{province_id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProvince(
        [FromRoute] Guid province_id,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var province = await context.GeographicBoundaries
            .OfType<Province>()
            .SingleOrDefaultAsync(item => item.Id == province_id, cancellationToken);
        if (province == null)
        {
            return NotFound();
        }

        context.Remove(province);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
