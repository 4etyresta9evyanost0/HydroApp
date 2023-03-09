using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Batch
{
    public int Id { get; set; }

    public int? DetailsDesiredAmount { get; set; }

    public int DetailsMadeAmount { get; set; }

    public DateTime? RequestDate { get; set; }

    public DateTime? CompletionDate { get; set; }

    public int? Foreman { get; set; }

    public int? Detail { get; set; }

    public virtual Detail? DetailNavigation { get; set; }

    public virtual Foreman? ForemanNavigation { get; set; }
}
