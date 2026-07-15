using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.GeographicBoundaryModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers.Parties;

public partial class GeographicBoundariesController
{
    [HttpGet("types")]
    [ProducesResponseType(typeof(List<GeographicBoundaryType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGeographicBoundaryTypes(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var geographicBoundaryTypes = await context.GeographicBoundaryTypes.ToListAsync(cancellationToken);

        return Ok(geographicBoundaryTypes);
    }

    [HttpPost("types")]
    [ProducesResponseType(typeof(GeographicBoundaryType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGeographicBoundaryType(CreateGeographicBoundaryType createGeographicBoundaryType,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var geographicBoundaryType = new GeographicBoundaryType
        {
            Id = Guid.NewGuid(),
            Name = createGeographicBoundaryType.Name,
            Code = createGeographicBoundaryType.Code
        };

        await context.AddAsync(geographicBoundaryType);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("GeographicBoundaryType created: {GeographicBoundaryTypeId}", geographicBoundaryType.Id);

        return CreatedAtAction(
            nameof(GetGeographicBoundaryType),
            new { geographic_boundary_type_id = geographicBoundaryType.Id },
            geographicBoundaryType);
    }

    [HttpGet("types/{geographic_boundary_type_id}")]
    [ProducesResponseType(typeof(GeographicBoundaryType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGeographicBoundaryType(Guid geographic_boundary_type_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var geographicBoundaryType = await context.GeographicBoundaryTypes.FindAsync(geographic_boundary_type_id, cancellationToken);
        if (geographicBoundaryType == null)
        {
            return NotFound();
        }

        return Ok(geographicBoundaryType);
    }

    [HttpPatch("types/{geographic_boundary_type_id}")]
    [ProducesResponseType(typeof(GeographicBoundaryType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchGeographicBoundaryType(Guid geographic_boundary_type_id,
            [FromBody] JsonPatchDocument<GeographicBoundaryType> patchDoc,
            CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var geographicBoundaryType = await context.GeographicBoundaryTypes.FindAsync(geographic_boundary_type_id, cancellationToken);
        if (geographicBoundaryType == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(geographicBoundaryType);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(geographicBoundaryType);
    }

    [HttpDelete("types/{geographic_boundary_type_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGeographicBoundaryType(Guid geographic_boundary_type_id,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var geographicBoundaryType = await context.GeographicBoundaryTypes
            .FindAsync(geographic_boundary_type_id, cancellationToken);
        if (geographicBoundaryType == null)
        {
            return NotFound();
        }

        context.GeographicBoundaryTypes.Remove(geographicBoundaryType);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
