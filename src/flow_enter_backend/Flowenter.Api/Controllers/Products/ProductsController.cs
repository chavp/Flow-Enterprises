using Flowenter.Products.IServices;
using Flowenter.Parties.Mappings;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Flowenter.Api.Controllers.Products;

[ApiController]
[Route("api/parties/enterprises/{enterpriseId:guid}/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsServices _productsServices;
    private readonly IDbContextFactory<PartiesContext> _partiesFactory;

    public ProductsController(
        IProductsServices productsServices,
        IDbContextFactory<PartiesContext> partiesFactory)
    {
        _productsServices = productsServices;
        _partiesFactory = partiesFactory;
    }

    [HttpGet("services")]
    [ProducesResponseType(typeof(List<EnterpriseServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var services = await _productsServices.GetServicesAsync(providerPartyId.Value, cancellationToken);
        return Ok(services);
    }

    [HttpGet("goods")]
    [ProducesResponseType(typeof(List<EnterpriseGoodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGoods([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var goods = await _productsServices.GetGoodsAsync(providerPartyId.Value, cancellationToken);
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
            var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
            if (!providerPartyId.HasValue)
            {
                return NotFound();
            }

            await _productsServices.CreateServiceAsync(providerPartyId.Value, payload, cancellationToken);
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
            var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
            if (!providerPartyId.HasValue)
            {
                return NotFound();
            }

            var updated = await _productsServices.UpdateServiceAsync(providerPartyId.Value, serviceId, payload, cancellationToken);
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
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var deleted = await _productsServices.DeleteServiceAsync(providerPartyId.Value, serviceId, cancellationToken);
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
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var categories = await _productsServices.GetFeatureCategoriesAsync(providerPartyId.Value, cancellationToken);
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
            var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
            if (!providerPartyId.HasValue)
            {
                return NotFound();
            }

            await _productsServices.CreateFeatureCategoryAsync(providerPartyId.Value, payload, cancellationToken);
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
            var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
            if (!providerPartyId.HasValue)
            {
                return NotFound();
            }

            var updated = await _productsServices.UpdateFeatureCategoryAsync(providerPartyId.Value, categoryId, payload, cancellationToken);
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
            var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
            if (!providerPartyId.HasValue)
            {
                return NotFound();
            }

            var deleted = await _productsServices.DeleteFeatureCategoryAsync(providerPartyId.Value, categoryId, cancellationToken);
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
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var applicabilities = await _productsServices.GetServiceFeatureApplicabilitiesAsync(providerPartyId.Value, serviceId, cancellationToken);
        return Ok(applicabilities);
    }

    [HttpGet("services/{serviceId:guid}/price-coponents")]
    [ProducesResponseType(typeof(List<EnterpriseServicePriceCoponentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServicePriceCoponents(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var priceCoponents = await _productsServices.GetServicePriceCoponentsAsync(providerPartyId.Value, serviceId, cancellationToken);
        return Ok(priceCoponents);
    }

    [HttpGet("{productId:guid}/feature-applicabilities")]
    [ProducesResponseType(typeof(List<EnterpriseServiceFeatureApplicabilityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductFeatureApplicabilities(
        [FromRoute] Guid enterpriseId,
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var applicabilities = await _productsServices.GetProductFeatureApplicabilitiesAsync(providerPartyId.Value, productId, cancellationToken);
        return Ok(applicabilities);
    }

    [HttpGet("features")]
    [ProducesResponseType(typeof(List<EnterpriseProductFeatureDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatures([FromRoute] Guid enterpriseId, CancellationToken cancellationToken)
    {
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var features = await _productsServices.GetFeaturesAsync(providerPartyId.Value, cancellationToken);
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
            var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
            if (!providerPartyId.HasValue)
            {
                return NotFound();
            }

            await _productsServices.CreateFeatureAsync(providerPartyId.Value, payload, cancellationToken);
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
            var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
            if (!providerPartyId.HasValue)
            {
                return NotFound();
            }

            var updated = await _productsServices.UpdateFeatureAsync(providerPartyId.Value, featureId, payload, cancellationToken);
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
        var providerPartyId = await ResolveProviderPartyIdAsync(enterpriseId, cancellationToken);
        if (!providerPartyId.HasValue)
        {
            return NotFound();
        }

        var deleted = await _productsServices.DeleteFeatureAsync(providerPartyId.Value, featureId, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private async Task<Guid?> ResolveProviderPartyIdAsync(Guid enterpriseRoleId, CancellationToken cancellationToken)
    {
        using var context = _partiesFactory.CreateDbContext();
        return await context.PartyRoles
            .OfType<Enterprise>()
            .Where(item => item.Id == enterpriseRoleId && item.PartyId.HasValue)
            .Select(item => item.PartyId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
