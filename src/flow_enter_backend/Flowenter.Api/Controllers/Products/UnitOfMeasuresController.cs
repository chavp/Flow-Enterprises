using Flowenter.Products.Mappings;
using Flowenter.Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers.Products;

[ApiController]
[Route("api/products/unit-of-measures")]
public class UnitOfMeasuresController : ControllerBase
{
    private readonly IDbContextFactory<ProductsContext> _factory;

    public UnitOfMeasuresController(IDbContextFactory<ProductsContext> factory)
    {
        _factory = factory;
    }

    [HttpGet("currency-measures")]
    [ProducesResponseType(typeof(List<CurrencyMeasureDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrencyMeasures(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var data = await context.UnitOfMeasures
            .OfType<CurrencyMeasure>()
            .OrderBy(item => item.Abbreviation)
            .Select(item => new CurrencyMeasureDto
            {
                CurrencyMeasureId = item.Id!.Value,
                Abbreviation = item.Abbreviation!,
                Description = item.Description,
                CreatedAtUtc = item.CreatedAtUtc,
                UpdatedAtUtc = item.UpdatedAtUtc,
                Revision = item.Revision
            })
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpPost("currency-measures")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCurrencyMeasure(
        [FromBody] CreateCurrencyMeasureDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var abbreviation = NormalizeAbbreviation(payload.Abbreviation);

            using var context = _factory.CreateDbContext();

            var data = new CurrencyMeasure
            {
                Id = Guid.NewGuid(),
                Abbreviation = abbreviation,
                Description = NormalizeDescription(payload.Description)
            };

            await context.AddAsync(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetCurrencyMeasures), null, null);
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpPut("currency-measures/{currencyMeasureId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCurrencyMeasure(
        [FromRoute] Guid currencyMeasureId,
        [FromBody] UpdateCurrencyMeasureDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var abbreviation = NormalizeAbbreviation(payload.Abbreviation);

            using var context = _factory.CreateDbContext();

            var data = await context.UnitOfMeasures
                .OfType<CurrencyMeasure>()
                .FirstOrDefaultAsync(item => item.Id == currencyMeasureId, cancellationToken);
            if (data == null)
            {
                return NotFound();
            }

            data.Abbreviation = abbreviation;
            data.Description = NormalizeDescription(payload.Description);
            await context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpGet("time-frequency-measures")]
    [ProducesResponseType(typeof(List<TimeFrequencyMeasureDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeFrequencyMeasures(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var data = await context.UnitOfMeasures
            .OfType<TimeFrequencyMeasure>()
            .OrderBy(item => item.Abbreviation)
            .Select(item => new TimeFrequencyMeasureDto
            {
                TimeFrequencyMeasureId = item.Id!.Value,
                Abbreviation = item.Abbreviation!,
                Description = item.Description,
                CreatedAtUtc = item.CreatedAtUtc,
                UpdatedAtUtc = item.UpdatedAtUtc,
                Revision = item.Revision
            })
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpPost("time-frequency-measures")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTimeFrequencyMeasure(
        [FromBody] CreateTimeFrequencyMeasureDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var abbreviation = NormalizeAbbreviation(payload.Abbreviation);

            using var context = _factory.CreateDbContext();

            var data = new TimeFrequencyMeasure
            {
                Id = Guid.NewGuid(),
                Abbreviation = abbreviation,
                Description = NormalizeDescription(payload.Description)
            };

            await context.AddAsync(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetTimeFrequencyMeasures), null, null);
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpPut("time-frequency-measures/{timeFrequencyMeasureId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTimeFrequencyMeasure(
        [FromRoute] Guid timeFrequencyMeasureId,
        [FromBody] UpdateTimeFrequencyMeasureDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var abbreviation = NormalizeAbbreviation(payload.Abbreviation);

            using var context = _factory.CreateDbContext();

            var data = await context.UnitOfMeasures
                .OfType<TimeFrequencyMeasure>()
                .FirstOrDefaultAsync(item => item.Id == timeFrequencyMeasureId, cancellationToken);
            if (data == null)
            {
                return NotFound();
            }

            data.Abbreviation = abbreviation;
            data.Description = NormalizeDescription(payload.Description);
            await context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    private static string NormalizeAbbreviation(string? abbreviation)
    {
        var normalized = abbreviation?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Abbreviation is required.");
        }

        return normalized;
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}

public sealed class CurrencyMeasureDto
{
    public Guid CurrencyMeasureId { get; set; }
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong Revision { get; set; }
}

public sealed class CreateCurrencyMeasureDto
{
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateCurrencyMeasureDto
{
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class TimeFrequencyMeasureDto
{
    public Guid TimeFrequencyMeasureId { get; set; }
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong Revision { get; set; }
}

public sealed class CreateTimeFrequencyMeasureDto
{
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateTimeFrequencyMeasureDto
{
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
}
