using Flowenter.Domain.Models;
using Flowenter.Parties.IServices;
using Flowenter.Parties.IServices.Dtos.ContactMechanismDto;
using Flowenter.Parties.Mappings;
using Flowenter.Parties.Models.ContactMechanismModels;
using Flowenter.Parties.Services.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowenter.Parties.Services;

public class ContactMechanismServices: IContactMechanismServices
{
    private readonly IDbContextFactory<PartiesContext> _factory;
    private readonly ILogger<ContactMechanismServices> _logger;
    public ContactMechanismServices(IDbContextFactory<PartiesContext> factory, ILogger<ContactMechanismServices> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ResultT<ContactMechanismTypeDto>> CreateContactMechanismTypeAsync(CreateContactMechanismType createContactMechanismType, CancellationToken cancellationToken)
    {
        try
        {
            using (var context = _factory.CreateDbContext())
            {
                var contactMechanismType = new ContactMechanismType
                {
                    Id = Guid.NewGuid(),
                    Name = createContactMechanismType.Name,
                    Code = createContactMechanismType.Code
                };

                await context.AddAsync(contactMechanismType);
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("ContactMechanismType created: {ContactMechanismTypeId}", contactMechanismType.Id);

                return ResultT<ContactMechanismTypeDto>.Success(contactMechanismType.ToDto());
            }
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database update error while creating ContactMechanismType");
            return ResultT<ContactMechanismTypeDto>
                .Failure(Error.Duplicate("A database error occurred while creating the ContactMechanismType."));
        }
    }

    public async Task<Result> DeleteContactMechanismTypeAsync(Guid contactMechanismTypeId, CancellationToken cancellationToken)
    {
        using (var context = _factory.CreateDbContext())
        {
            var contactMechanismType = await context.ContactMechanismTypes
                .FindAsync(contactMechanismTypeId, cancellationToken);
            if (contactMechanismType == null)
            {
                return Result.Failure(Error.NotFound("Not found contactMechanismType"));
            }

            context.ContactMechanismTypes.Remove(contactMechanismType);
            var effect = await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }

    public async Task<Maybe<ContactMechanismTypeDto>> GetContactMechanismTypeAsync(Guid contactMechanismTypeId, CancellationToken cancellationToken)
    {
        using (var context = _factory.CreateDbContext())
        {
            var contactMechanismType = await context.ContactMechanismTypes
                .FindAsync(contactMechanismTypeId, cancellationToken);
            if (contactMechanismType == null)
            {
                return Maybe<ContactMechanismTypeDto>.None;
            }

            return Maybe<ContactMechanismTypeDto>.Some(contactMechanismType.ToDto());
        }
    }

    public async Task<IReadOnlyList<ContactMechanismTypeDto>> GetContactMechanismTypesAsync(CancellationToken cancellationToken)
    {
        using (var context = _factory.CreateDbContext())
        {
            var contactMechanismTypes = await context
                .ContactMechanismTypes
                .ToListAsync(cancellationToken);
            return contactMechanismTypes.Select(cmt => cmt.ToDto()).ToList();
        }
    }

    public async Task<ResultT<ContactMechanismTypeDto>> PatchContactMechanismTypeAsync(Guid contact_mechanism_type_id, JsonPatchDocument<ContactMechanismType> patchDoc, CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            //throw new ArgumentNullException(nameof(patchDoc));
            return ResultT<ContactMechanismTypeDto>
                .Failure(Error.ArgumentNull(nameof(patchDoc)));
        }

        try
        {
            using (var context = _factory.CreateDbContext())
            {
                var contactMechanismType = await context.ContactMechanismTypes
                    .FindAsync(contact_mechanism_type_id, cancellationToken);
                if (contactMechanismType == null)
                {
                    return ResultT<ContactMechanismTypeDto>
                        .Failure(Error.NotFound("ContactMechanismType is null."));
                }

                patchDoc.ApplyTo(contactMechanismType);

                await context.SaveChangesAsync(cancellationToken);

                return ResultT<ContactMechanismTypeDto>
                    .Success(contactMechanismType.ToDto());
            }
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database update error while creating ContactMechanismType");
            return ResultT<ContactMechanismTypeDto>
                .Failure(Error.Duplicate("A database error occurred while creating the ContactMechanismType."));
        }
    }
}
