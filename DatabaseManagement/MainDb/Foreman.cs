using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Foreman
{
    public int Id { get; set; }

    public int Workshop { get; set; }

    public virtual ICollection<Batch> Batches { get; } = new List<Batch>();

    public virtual Employee IdNavigation { get; set; } = null!;

    public virtual Workshop WorkshopNavigation { get; set; } = null!;
}
