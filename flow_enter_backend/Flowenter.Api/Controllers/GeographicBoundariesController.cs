using Flowenter.Parties.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers
{
    [ApiController]
    [Route("api/geographic-boundaries")]
    public partial class GeographicBoundariesController : ControllerBase
    {
        private readonly ILogger<GeographicBoundariesController> _logger;
        private readonly IDbContextFactory<PartiesContext> _factory;

        public GeographicBoundariesController(ILogger<GeographicBoundariesController> logger,
            IDbContextFactory<PartiesContext> factory)
        {
            _logger = logger;
            _factory = factory;
        }
    }
}
