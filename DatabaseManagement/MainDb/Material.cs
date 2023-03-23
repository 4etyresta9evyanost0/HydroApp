using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Material
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public double Amount { get; set; }

    public string Unit { get; set; } = null!;

    public virtual ICollection<MaterialsForDetail> MaterialsForDetails { get; } = new List<MaterialsForDetail>();

    public virtual ICollection<SupplyDetail> SupplyDetails { get; } = new List<SupplyDetail>();
}
