using Flowenter.Domain.Models;
using Flowenter.Parties.IServices.Dtos.ContactMechanismDto;
using Flowenter.Parties.Models.ContactMechanismModels;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.IServices;

public interface IContactMechanismServices
{
    Task<IReadOnlyList<ContactMechanismTypeDto>> GetContactMechanismTypesAsync(CancellationToken cancellationToken);
    Task<ResultT<ContactMechanismTypeDto>> CreateContactMechanismTypeAsync(
        CreateContactMechanismType createContactMechanismType
            , CancellationToken cancellationToken);

    Task<Maybe<ContactMechanismTypeDto>> GetContactMechanismTypeAsync(Guid contactMechanismTypeId, CancellationToken cancellationToken);
    Task<ResultT<ContactMechanismTypeDto>> PatchContactMechanismTypeAsync(Guid contactMechanismTypeId
        , JsonPatchDocument<ContactMechanismType> patchDoc
        , CancellationToken cancellationToken);

    Task<Result> DeleteContactMechanismTypeAsync(Guid contactMechanismTypeId
    , CancellationToken cancellationToken);
}
