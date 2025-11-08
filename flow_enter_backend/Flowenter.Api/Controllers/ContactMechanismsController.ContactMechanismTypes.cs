using Flowenter.Parties.IServices.Dtos;
using Flowenter.Parties.Models.ContactMechanismModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Controllers;

public partial class ContactMechanismsController
{
    [HttpGet("types")]
    [ProducesResponseType(typeof(List<ContactMechanismType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContactMechanismTypes(CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var contactMechanismTypes = await context.ContactMechanismTypes.ToListAsync(cancellationToken);

        return Ok(contactMechanismTypes);
    }

    [HttpPost("types")]
    [ProducesResponseType(typeof(ContactMechanismType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContactMechanismType(CreateContactMechanismType createContactMechanismType
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var contactMechanismType = new ContactMechanismType
        {
            Id = Guid.NewGuid(),
            Name = createContactMechanismType.Name,
            Code = createContactMechanismType.Code
        };

        await context.AddAsync(contactMechanismType);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ContactMechanismType created: {ContactMechanismTypeId}", contactMechanismType.Id);

        return CreatedAtAction(
            nameof(GetContactMechanismType),
            new { contact_mechanism_type_id = contactMechanismType.Id },
            contactMechanismType);
    }

    [HttpGet("types/{contact_mechanism_type_id}")]
    [ProducesResponseType(typeof(ContactMechanismType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContactMechanismType(Guid contact_mechanism_type_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var contactMechanismType = await context.ContactMechanismTypes
            .FindAsync(contact_mechanism_type_id, cancellationToken);
        if (contactMechanismType == null)
        {
            return NotFound();
        }

        return Ok(contactMechanismType);
    }

    [HttpPatch("types/{contact_mechanism_type_id}")]
    [ProducesResponseType(typeof(ContactMechanismType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchContactMechanismType(Guid contact_mechanism_type_id
        , [FromBody] JsonPatchDocument<ContactMechanismType> patchDoc
        , CancellationToken cancellationToken)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        using var context = _factory.CreateDbContext();

        var contactMechanismType = await context.ContactMechanismTypes
            .FindAsync(contact_mechanism_type_id, cancellationToken);
        if (contactMechanismType == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(contactMechanismType);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(contactMechanismType);
    }

    [HttpDelete("types/{contact_mechanism_type_id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContactMechanismType(Guid contact_mechanism_type_id
        , CancellationToken cancellationToken)
    {
        using var context = _factory.CreateDbContext();

        var contactMechanismType = await context.ContactMechanismTypes
            .FindAsync(contact_mechanism_type_id, cancellationToken);
        if (contactMechanismType == null)
        {
            return NotFound();
        }

        context.ContactMechanismTypes.Remove(contactMechanismType);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
