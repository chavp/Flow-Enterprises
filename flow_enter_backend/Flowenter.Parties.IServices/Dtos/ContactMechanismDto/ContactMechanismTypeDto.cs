using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.IServices.Dtos.ContactMechanismDto;

public record ContactMechanismTypeDto
{
    public Guid Id { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
}
