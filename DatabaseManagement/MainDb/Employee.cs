using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Employee
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string? Patronym { get; set; }

    public string Occupation { get; set; } = null!;

    public string? Adress { get; set; }

    public long? Phonenum { get; set; }

    public int? Passport { get; set; }

    public virtual Designer? Designer { get; set; }

    public virtual Foreman? Foreman { get; set; }
}
