using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Models;

public sealed class PostalAddress : ContactMechanism
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }

    public string? House { get; set; }
    public string? HouseNumber { get; set; }
    public string? Address { get; set; }
    public string? District { get; set; }
    public string? SubDistrict { get; set; }
    public string? Province { get; set; }
}
