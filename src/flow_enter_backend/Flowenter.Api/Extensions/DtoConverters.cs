using Flowenter.Parties.IServices.Dtos.EnterpriseDto;
using Flowenter.Parties.Models.PartyModels;

namespace Flowenter.Api.Extensions;

public static class DtoConverters
{
    public static EnterpriseDto ToDto(this Enterprise enterprise)
    {
        return new EnterpriseDto
        {
            EnterpriseId = enterprise.Id.Value,
            //TenantId = enterprise.TenantId.Value,
            LegalName = enterprise.LegalName,
            Information = enterprise.Information,
            Logo = enterprise.Logo,
            BrandName = enterprise.BrandName,
            Notes = enterprise.Notes,
            LegalStructureId = enterprise.LegalStructureId,
            BusinessRegistrationNumber = enterprise.BusinessRegistrationNumber,
            TaxId = enterprise.TaxId,
            FiscalYearStartMonth = enterprise.FiscalYearStartMonth,
            FromDate = enterprise.FromDate,
            ThruDate = enterprise.ThruDate,
            CreatedAtUtc = enterprise.CreatedAtUtc,
            UpdatedAtUtc = enterprise.UpdatedAtUtc,
            CreatedBy = enterprise.CreatedBy,
            UpdatedBy = enterprise.UpdatedBy,
            Revision = enterprise.Revision
        };
    }
}
