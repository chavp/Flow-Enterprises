using Flowenter.Parties.IServices.Dtos.ContactMechanismDto;
using Flowenter.Parties.Models.ContactMechanismModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Services.Extensions;

public static class DtoConvertersExtensions
{
    public static ContactMechanismTypeDto ToDto(this ContactMechanismType entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return new ContactMechanismTypeDto
        {
            Id = entity.Id.Value,
            Code = entity.Code,
            Name = entity.Name
        };
    }
}
