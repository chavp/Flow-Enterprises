using Flowenter.Domain.Models;
using Flowenter.Parties.IServices.Dtos.ContactMechanismDto;
using Flowenter.Parties.Models.ContactMechanismModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers.Parties;

public partial class ContactMechanismsController
{
    [HttpGet("types")]
    [ProducesResponseType(typeof(IReadOnlyList<ContactMechanismTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContactMechanismTypes(CancellationToken cancellationToken)
    {
        var result = await _contactMechanismServices
            .GetContactMechanismTypesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("types")]
    [ProducesResponseType(typeof(ContactMechanismTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContactMechanismType(CreateContactMechanismType createContactMechanismType
        , CancellationToken cancellationToken)
            => _contactMechanismServices
                    .CreateContactMechanismTypeAsync(createContactMechanismType, cancellationToken)
                    .Match<IActionResult, ContactMechanismTypeDto>(
                        onSuccess: resp => CreatedAtAction(
                            nameof(GetContactMechanismType),
                            new { contact_mechanism_type_id = resp.Id },
                            resp),
                        onFailure: BadRequest
                     );

    [HttpGet("types/{contact_mechanism_type_id}")]
    [ProducesResponseType(typeof(ContactMechanismTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContactMechanismType([FromRoute] Guid contact_mechanism_type_id
        , CancellationToken cancellationToken)
            => _contactMechanismServices.GetContactMechanismTypeAsync(
                 contact_mechanism_type_id, cancellationToken)
                 .Match<IActionResult, ContactMechanismTypeDto>(Ok, NotFound);

    [HttpPatch("types/{contact_mechanism_type_id}")]
    [ProducesResponseType(typeof(ContactMechanismTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchContactMechanismType([FromRoute] Guid contact_mechanism_type_id
        , [FromBody] JsonPatchDocument<ContactMechanismType> patchDoc
        , CancellationToken cancellationToken)
        => _contactMechanismServices
            .PatchContactMechanismTypeAsync(contact_mechanism_type_id, patchDoc, cancellationToken)
            .Match<IActionResult, ContactMechanismTypeDto>(Ok, BadRequest);

    [HttpDelete("types/{contact_mechanism_type_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContactMechanismType([FromRoute] Guid contact_mechanism_type_id
        , CancellationToken cancellationToken)
    {
        return _contactMechanismServices
            .DeleteContactMechanismTypeAsync(contact_mechanism_type_id, cancellationToken)
            .Match<IActionResult>(NoContent, NotFound);
    }
}
