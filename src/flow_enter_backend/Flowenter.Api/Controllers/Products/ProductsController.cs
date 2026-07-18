using Flowenter.Products.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Flowenter.Api.Controllers.Products;

[ApiController]
[Route("api/parties/enterprises/{enterpriseId:guid}/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsServices _productsServices;

    public ProductsController(IProductsServices productsServices)
    {
        _productsServices = productsServices;
    }

    [HttpGet("services")]
    [ProducesResponseType(typeof(List<EnterpriseServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        var services = await _productsServices.GetServicesAsync(enterpriseId, cancellationToken);
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
        try
        {
            await _productsServices.CreateServiceAsync(enterpriseId, payload, cancellationToken);
            return CreatedAtAction(nameof(GetServices), new { enterpriseId }, null);
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
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
        try
        {
            var updated = await _productsServices.UpdateServiceAsync(enterpriseId, serviceId, payload, cancellationToken);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpDelete("services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteService(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var deleted = await _productsServices.DeleteServiceAsync(enterpriseId, serviceId, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
