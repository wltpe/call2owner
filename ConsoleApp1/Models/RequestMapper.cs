using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class RequestMapper
{
    public int RequestId { get; set; }

    public string RequestUrl { get; set; } = null!;

    public string RequestParameterDetail { get; set; } = null!;

    public bool IsActive { get; set; }
}
