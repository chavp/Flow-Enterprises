using Flowenter.Domain.Models;
using Flowenter.Parties.IServices.Dtos.EnterpriseDto;
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


        [HttpPost("enterprises")]
        [ProducesResponseType(typeof(Enterprise), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEnterprise(CreateEnterpriseDto createDto
            , CancellationToken cancellationToken)
        {
            using var context = _factory.CreateDbContext();
            using var tran = context.Database.BeginTransaction();

            var partyType = await context.PartyTypes
                .SingleAsync(pt => pt.Code == PartyType.Organization);

            var org = new Organization
            {
                Id = Guid.NewGuid(),
                Name = createDto.LegalName,
                Type = partyType
            };
            await context.AddAsync(org);
            await context.SaveChangesAsync(cancellationToken);

            var roleType = await context.PartyRoleTypes
                .SingleAsync(pt => pt.Code == PartyRoleType.Enterprise, cancellationToken);

            // Map DTO to entity
            var newData = new Enterprise
            {
                Id = Guid.NewGuid(),
                Type = roleType,
                PartyId = org.Id,

                LegalName = createDto.LegalName,
                Information = createDto.Information,
                BrandName = createDto.BrandName,
                Logo = createDto.Logo,
                Notes = createDto.Notes,
                LegalStructureId = createDto.LegalStructureId,
                BusinessRegistrationNumber = createDto.BusinessRegistrationNumber,
                TaxId = createDto.TaxId,
                FiscalYearStartMonth = createDto.FiscalYearStartMonth
            };

            await context.AddAsync(newData);
            await context.SaveChangesAsync(cancellationToken);
            await tran.CommitAsync();

            _logger.LogInformation("Enterprise created: {PartyRoleId}", newData.Id);

            return CreatedAtAction(
                nameof(GetPartyRole),
                new { party_role_id = newData.Id },
                newData);
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
