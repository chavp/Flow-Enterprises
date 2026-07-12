using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(FirstName), nameof(MiddleName), nameof(LastName))]
public sealed class PersonName : EffectiveEntity
{
    protected PersonName() { }
    public PersonName(Person? person, string? firstName, string? lastName, Language? language)
    {
        Person = person;
        FirstName = firstName;
        LastName = lastName;
        Language = language;
    }

    public Guid? PersonId { get; set; }
    public Person? Person { get; set; }

    [Required, StringLength(300)]
    public string? FirstName { get; set; }

    [StringLength(300)]
    public string? MiddleName { get; set; }

    [Required, StringLength(500)]
    public string? LastName { get; set; }

    [Required]
    public Guid? LanguageId { get; set; }
    public Language? Language { get; set; }
}
