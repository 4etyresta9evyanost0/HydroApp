using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Workshop
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Foreman> Foremen { get; } = new List<Foreman>();
}
