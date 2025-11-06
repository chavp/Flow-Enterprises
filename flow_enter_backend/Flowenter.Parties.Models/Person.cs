using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Models;

public class Person : Party
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
}
