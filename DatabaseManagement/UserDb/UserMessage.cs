using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class UserMessage
{
    public int UserMessageId { get; set; }

    public int Sender { get; set; }

    public int Receiver { get; set; }

    public DateTime SendedDate { get; set; }

    public DateTime? RedactedDate { get; set; }

    public virtual User ReceiverNavigation { get; set; } = null!;

    public virtual User SenderNavigation { get; set; } = null!;
}
