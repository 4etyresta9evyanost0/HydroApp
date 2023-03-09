using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class UserMessagesContent
{
    public int UserMessageId { get; set; }

    public string? Content { get; set; }

    public virtual UserMessage UserMessage { get; set; } = null!;
}
