using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class ResourceMapper
{
    public int ResourceId { get; set; }

    public string ResourceName { get; set; } = null!;

    public int ProviderId { get; set; }

    public string BaseUrl { get; set; } = null!;

    public string? AdditionalUrl { get; set; }

    public string Endpoint { get; set; } = null!;

    public string Verb { get; set; } = null!;

    public string RequestParameterDetail { get; set; } = null!;

    public string ResponseParameterDetail { get; set; } = null!;

    public bool IsActive { get; set; }
}
