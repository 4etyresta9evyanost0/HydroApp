using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class Commission
{
    public int Id { get; set; }

    public DateTime CommissionDate { get; set; }

    public DateTime? ExecutionDate { get; set; }

    public int Client { get; set; }

    public virtual Client ClientNavigation { get; set; } = null!;

    public virtual ICollection<CommissionDetail> CommissionDetails { get; } = new List<CommissionDetail>();


    public bool IsExecutionOver
    {
        get => ExecutionDate.HasValue ? true : ExecutionDate > DateTime.Today;
    }
}
