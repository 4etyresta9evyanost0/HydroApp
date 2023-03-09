using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Designer
{
    public int Id { get; set; }

    public virtual ICollection<Construction> Constructions { get; } = new List<Construction>();

    public virtual Employee IdNavigation { get; set; } = null!;
}
