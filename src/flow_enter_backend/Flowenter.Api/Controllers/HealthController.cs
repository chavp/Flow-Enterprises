using Flowenter.Parties.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Flowenter.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    private readonly IDbContextFactory<PartiesContext> _factory;

    public HealthController(IDbContextFactory<PartiesContext> factory)
    {
        _factory = factory;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();
        context.Database.Migrate();

        var canConnect = await context.Database.CanConnectAsync(cancellationToken);
        context.SaveChanges();

        return canConnect
            ? Ok(new { status = "Healthy" })
            : StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}
