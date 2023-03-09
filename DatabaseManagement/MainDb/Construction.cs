using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Construction
{
    public int Id { get; set; }

    public int? Developer { get; set; }

    public int Amount { get; set; }

    public decimal? Price { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<CommissionDetail> CommissionDetails { get; } = new List<CommissionDetail>();

    public virtual Detail? Detail { get; set; }

    public virtual Designer? DeveloperNavigation { get; set; }

    public virtual Production? Production { get; set; }
}
