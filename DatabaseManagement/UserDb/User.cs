using System;
using System.Collections.Generic;

namespace HydroApp;

public partial class User
{
    public int Id { get; set; }

    public string Nickname { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte Type { get; set; }

    public virtual ICollection<UserMessage> UserMessageReceiverNavigations { get; } = new List<UserMessage>();

    public virtual ICollection<UserMessage> UserMessageSenderNavigations { get; } = new List<UserMessage>();
}
