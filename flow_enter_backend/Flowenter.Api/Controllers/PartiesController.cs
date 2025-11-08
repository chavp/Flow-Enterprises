using Flowenter.Parties.Mappings;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers
{
    [ApiController]
    [Route("api/parties")]
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

        [HttpPost("roles/{party_role_id}")]
        [ProducesResponseType(typeof(Enterprise), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPartyRole(Guid party_role_id
            , CancellationToken cancellationToken)
        {

            using var context = _factory.CreateDbContext();
            var partyRole = await context.PartyRoles
                .Include(pr => pr.Party)
                .Include(pr => pr.Type)
                .FirstOrDefaultAsync(pr => pr.Id == party_role_id, cancellationToken);
            if (partyRole == null)
            {
                return NotFound();
            }
            return Ok(partyRole);
        }
    }
}
