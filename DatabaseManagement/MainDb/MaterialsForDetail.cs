using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class MaterialsForDetail
{
    public int IdDetail { get; set; }

    public int IdMaterial { get; set; }

    public int MaterialsAmount { get; set; }

    public virtual Detail IdDetailNavigation { get; set; } = null!;

    public virtual Material IdMaterialNavigation { get; set; } = null!;
}
