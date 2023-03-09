using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Adress { get; set; } = null!;

    public long Ogrn { get; set; }

    public long Inn { get; set; }

    public int? Kpp { get; set; }

    public virtual ICollection<Commission> Commissions { get; } = new List<Commission>();
}
