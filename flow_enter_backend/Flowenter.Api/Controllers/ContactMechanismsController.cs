using Flowenter.Parties.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers
{
    [ApiController]
    [Route("api/contact-mechanisms")]
    public partial class ContactMechanismsController : ControllerBase
    {
        private readonly ILogger<ContactMechanismsController> _logger;
        private readonly IDbContextFactory<PartiesContext> _factory;

        public ContactMechanismsController(ILogger<ContactMechanismsController> logger,
            IDbContextFactory<PartiesContext> factory)
        {
            _logger = logger;
            _factory = factory;
        }
    }
}
