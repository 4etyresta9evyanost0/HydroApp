using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Supply
{
    public int Id { get; set; }

    public DateTime CommissionDate { get; set; }

    public DateTime? SupplyDate { get; set; }

    public int Supplier { get; set; }

    public virtual Supplier SupplierNavigation { get; set; } = null!;

    public virtual ICollection<SupplyDetail> SupplyDetails { get; } = new List<SupplyDetail>();
}
