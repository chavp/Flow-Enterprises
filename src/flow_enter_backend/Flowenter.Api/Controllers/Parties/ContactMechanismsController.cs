using Flowenter.Parties.IServices;
using Flowenter.Parties.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers.Parties
{
    [ApiController]
    [Route("api/contact-mechanisms")]
    public partial class ContactMechanismsController : ControllerBase
    {
        private readonly ILogger<ContactMechanismsController> _logger;
        private readonly IDbContextFactory<PartiesContext> _factory;
        private readonly IContactMechanismServices _contactMechanismServices;

        public ContactMechanismsController(ILogger<ContactMechanismsController> logger,
            IDbContextFactory<PartiesContext> factory, 
            IContactMechanismServices contactMechanismServices) 
        {
            _logger = logger;
            _factory = factory;
            _contactMechanismServices = contactMechanismServices;
        }
    }
}
