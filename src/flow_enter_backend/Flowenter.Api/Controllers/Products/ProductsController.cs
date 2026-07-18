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

    [HttpGet("goods")]
    [ProducesResponseType(typeof(List<EnterpriseGoodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGoods([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        var goods = await _productsServices.GetGoodsAsync(enterpriseId, cancellationToken);
        return Ok(goods);
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

    [HttpGet("feature-categories")]
    [ProducesResponseType(typeof(List<ProductFeatureCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatureCategories([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        var categories = await _productsServices.GetFeatureCategoriesAsync(enterpriseId, cancellationToken);
        return Ok(categories);
    }

    [HttpPost("feature-categories")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFeatureCategory(
        [FromRoute] Guid enterpriseId,
        [FromBody] CreateProductFeatureCategoryDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            await _productsServices.CreateFeatureCategoryAsync(enterpriseId, payload, cancellationToken);
            return CreatedAtAction(nameof(GetFeatureCategories), new { enterpriseId }, null);
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpPut("feature-categories/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFeatureCategory(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid categoryId,
        [FromBody] UpdateProductFeatureCategoryDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _productsServices.UpdateFeatureCategoryAsync(enterpriseId, categoryId, payload, cancellationToken);
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

    [HttpDelete("feature-categories/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFeatureCategory(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _productsServices.DeleteFeatureCategoryAsync(enterpriseId, categoryId, cancellationToken);
            if (!deleted)
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

    [HttpGet("feature-types")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatureTypes([FromRoute] Guid enterpriseId)
    {
        _ = enterpriseId;
        var types = await _productsServices.GetFeatureTypesAsync();
        return Ok(types);
    }

    [HttpGet("feature-applicability-types")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatureApplicabilityTypes([FromRoute] Guid enterpriseId)
    {
        _ = enterpriseId;
        var types = await _productsServices.GetFeatureApplicabilityTypesAsync();
        return Ok(types);
    }

    [HttpGet("services/{serviceId:guid}/feature-applicabilities")]
    [ProducesResponseType(typeof(List<EnterpriseServiceFeatureApplicabilityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServiceFeatureApplicabilities(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var applicabilities = await _productsServices.GetServiceFeatureApplicabilitiesAsync(enterpriseId, serviceId, cancellationToken);
        return Ok(applicabilities);
    }

    [HttpGet("services/{serviceId:guid}/price-coponents")]
    [ProducesResponseType(typeof(List<EnterpriseServicePriceCoponentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServicePriceCoponents(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var priceCoponents = await _productsServices.GetServicePriceCoponentsAsync(enterpriseId, serviceId, cancellationToken);
        return Ok(priceCoponents);
    }

    [HttpGet("{productId:guid}/feature-applicabilities")]
    [ProducesResponseType(typeof(List<EnterpriseServiceFeatureApplicabilityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductFeatureApplicabilities(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var applicabilities = await _productsServices.GetProductFeatureApplicabilitiesAsync(enterpriseId, productId, cancellationToken);
        return Ok(applicabilities);
    }

    [HttpGet("features")]
    [ProducesResponseType(typeof(List<EnterpriseProductFeatureDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatures([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        var features = await _productsServices.GetFeaturesAsync(enterpriseId, cancellationToken);
        return Ok(features);
    }

    [HttpPost("features")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFeature(
        [FromRoute] Guid enterpriseId,
        [FromBody] CreateEnterpriseProductFeatureDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            await _productsServices.CreateFeatureAsync(enterpriseId, payload, cancellationToken);
            return CreatedAtAction(nameof(GetFeatures), new { enterpriseId }, null);
        }
        catch (ArgumentException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpPut("features/{featureId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFeature(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid featureId,
        [FromBody] UpdateEnterpriseProductFeatureDto payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _productsServices.UpdateFeatureAsync(enterpriseId, featureId, payload, cancellationToken);
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

    [HttpDelete("features/{featureId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFeature(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid featureId,
        CancellationToken cancellationToken)
    {
        var deleted = await _productsServices.DeleteFeatureAsync(enterpriseId, featureId, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
