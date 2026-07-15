using Flowenter.Parties.Mappings;
using Flowenter.Products.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowenter.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    private readonly IDbContextFactory<PartiesContext> _partiesFactory;
    private readonly IDbContextFactory<ProductsContext> _productsFactory;
    public HealthController(
        IDbContextFactory<PartiesContext> partiesFactory,
        IDbContextFactory<ProductsContext> productsFactory)
    {
        _partiesFactory = partiesFactory;
        _productsFactory = productsFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        using var context = _partiesFactory.CreateDbContext();
        context.Database.Migrate();
        var canConnect = await context.Database.CanConnectAsync(cancellationToken);
        context.SaveChanges();

        using var prdContext = _productsFactory.CreateDbContext();
        prdContext.Database.Migrate();
        var canPrdConnect = await prdContext.Database.CanConnectAsync(cancellationToken);
        prdContext.SaveChanges();

        return canConnect
            ? Ok(new { status = "Healthy" })
            : StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}
