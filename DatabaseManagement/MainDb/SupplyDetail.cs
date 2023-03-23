using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class SupplyDetail
{
    public int IdSupply { get; set; }

    public int IdMaterial { get; set; }

    public double MaterialAmount { get; set; }

    public virtual Material IdMaterialNavigation { get; set; } = null!;

    public virtual Supply IdSupplyNavigation { get; set; } = null!;
}
