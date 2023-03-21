using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Production
{
    public int Id { get; set; }

    public string? BrandName { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<DetailsForProduction> DetailsForProductions { get; } = new List<DetailsForProduction>();

    public virtual Construction IdNavigation { get; set; } = null!;
}
