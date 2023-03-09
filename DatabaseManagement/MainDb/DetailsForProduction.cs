using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class DetailsForProduction
{
    public int IdProduction { get; set; }

    public int IdDetail { get; set; }

    public int DetailsAmount { get; set; }

    public virtual Detail IdDetailNavigation { get; set; } = null!;

    public virtual Production IdProductionNavigation { get; set; } = null!;
}
