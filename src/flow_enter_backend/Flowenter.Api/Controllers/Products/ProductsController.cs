using Flowenter.Products.Mappings;
using Flowenter.Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers.Products;

[ApiController]
[Route("api/parties/enterprises/{enterpriseId:guid}/products")]
public class ProductsController : ControllerBase
{
    private readonly IDbContextFactory<ProductsContext> _factory;

    public ProductsController(IDbContextFactory<ProductsContext> factory)
    {
        _factory = factory;
    }

    [HttpGet("services")]
    [ProducesResponseType(typeof(List<EnterpriseServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var services = await context.Products
            .OfType<Service>()
            .Where(item => item.ProviderPartyId == enterpriseId)
            .OrderBy(item => item.Name)
            .Select(item => new EnterpriseServiceDto
            {
                ServiceId = item.Id!.Value,
                EnterpriseId = enterpriseId,
                Name = item.Name!,
                Description = item.Description,
                CreatedAtUtc = item.CreatedAtUtc,
                UpdatedAtUtc = item.UpdatedAtUtc,
                Revision = item.Revision
            })
            .ToListAsync(cancellationToken);

        return Ok(services);
    }

    [HttpPost("services")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateService(
        [FromRoute] Guid enterpriseId,
        [FromBody] CreateEnterpriseServiceDto payload,
        CancellationToken cancellationToken)
    {
        var name = payload.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { message = "Service name is required." });
        }

        using var context = _factory.CreateDbContext();

        var data = new Service
        {
            Id = Guid.NewGuid(),
            ProviderPartyId = enterpriseId,
            Name = name,
            Description = string.IsNullOrWhiteSpace(payload.Description) ? null : payload.Description.Trim()
        };

        await context.AddAsync(data, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetServices), new { enterpriseId }, null);
    }

    [HttpPut("services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateService(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid serviceId,
        [FromBody] UpdateEnterpriseServiceDto payload,
        CancellationToken cancellationToken)
    {
        var name = payload.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { message = "Service name is required." });
        }

        using var context = _factory.CreateDbContext();

        var service = await context.Products
            .OfType<Service>()
            .FirstOrDefaultAsync(item => item.Id == serviceId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (service == null)
        {
            return NotFound();
        }

        service.Name = name;
        service.Description = string.IsNullOrWhiteSpace(payload.Description) ? null : payload.Description.Trim();

        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteService(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var service = await context.Products
            .OfType<Service>()
            .FirstOrDefaultAsync(item => item.Id == serviceId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (service == null)
        {
            return NotFound();
        }

        context.Remove(service);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public sealed class EnterpriseServiceDto
{
    public Guid ServiceId { get; set; }
    public Guid EnterpriseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong Revision { get; set; }
}

public sealed class CreateEnterpriseServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateEnterpriseServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
