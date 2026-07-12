using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record EnterprisesDto
{
    public List<EnterpriseDto> Data { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
