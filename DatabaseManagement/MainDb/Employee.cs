using System;
using System.Collections.Generic;
using System.Linq;

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

    public long? Passport { get; set; }

    public virtual Designer? Designer { get; set; }

    public virtual Foreman? Foreman { get; set; }

    public string FIO { get => Firstname.First() + ". " + (Patronym != null || Patronym != "" ? Patronym.First() + ". " : "") + Surname; }

    public string FullInfo { get => $"{Id}: {FIO}"; }

    public override string ToString() => FullInfo;
}
