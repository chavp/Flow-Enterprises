using Flowenter.Parties.Models.GeographicBoundaryModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.ContactMechanismModels;

public sealed class PostalAddress : ContactMechanism
{
    [Required]
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    [Required, StringLength(300)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? ZipCode { get; set; }


    [StringLength(300)]
    public string? Street { get; set; }
    
    [StringLength(300)]
    public string? City { get; set; }
    
    [StringLength(300)]
    public string? State { get; set; }


    [StringLength(100)]
    public string? House { get; set; }

    [StringLength(100)]
    public string? HouseNumber { get; set; }


    [StringLength(300)]
    public string? District { get; set; }

    [StringLength(300)]
    public string? SubDistrict { get; set; }

    [StringLength(300)]
    public string? Province { get; set; }
}
