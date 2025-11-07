using Flowenter.Parties.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class PartiesController : ControllerBase
    {
        private readonly ILogger<PartiesController> _logger;
        private readonly IDbContextFactory<PartiesContext> _factory;

        public PartiesController(ILogger<PartiesController> logger,
            IDbContextFactory<PartiesContext> factory)
        {
            _logger = logger;
            _factory = factory;
        }

    }
}
