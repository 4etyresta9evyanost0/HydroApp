using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Detail
{
    public int Id { get; set; }

    public string Purpose { get; set; } = null!;

    public float Width { get; set; }

    public float Height { get; set; }

    public float Length { get; set; }

    public virtual ICollection<Batch> Batches { get; } = new List<Batch>();

    public virtual ICollection<DetailsForProduction> DetailsForProductions { get; } = new List<DetailsForProduction>();

    public virtual Construction IdNavigation { get; set; } = null!;

    public virtual ICollection<MaterialsForDetail> MaterialsForDetails { get; } = new List<MaterialsForDetail>();
}
