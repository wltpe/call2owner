using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class Queue
{
    public int QueueId { get; set; }

    public string RequestJson { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
}
