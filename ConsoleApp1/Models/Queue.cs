using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Queue
{
    public int QueueId { get; set; }

    public string RequestJson { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
}
