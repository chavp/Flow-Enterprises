using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flowenter.Parties.Models.PartyModels;

[Table("People")]
public class Person : Party
{
    public Guid? GenderTypeId { get; set; }
    public GenderType? GenderType { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}