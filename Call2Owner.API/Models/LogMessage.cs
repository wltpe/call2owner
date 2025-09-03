using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class LogMessage
{
    public int Id { get; set; }

    public string? RequestUrl { get; set; }

    public string? RequestHeader { get; set; }

    public string? RequestQuerystring { get; set; }

    public string? RequestBody { get; set; }

    public string? Response { get; set; }

    public string? Ip { get; set; }

    public DateTime? CreatedOn { get; set; }
}
