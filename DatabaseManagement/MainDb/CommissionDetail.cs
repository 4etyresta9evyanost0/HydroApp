using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class CommissionDetail
{
    public int IdCommission { get; set; }

    public int IdConstruction { get; set; }

    public int ConstructionsAmount { get; set; }

    public virtual Commission IdCommissionNavigation { get; set; } = null!;

    public virtual Construction IdConstructionNavigation { get; set; } = null!;
}
