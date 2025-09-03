using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class RequestMapper
{
    public int RequestId { get; set; }

    public string RequestUrl { get; set; } = null!;

    public string RequestParameterDetail { get; set; } = null!;

    public bool IsActive { get; set; }
}
