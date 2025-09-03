using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Resource
{
    public int ResourceId { get; set; }

    public string ResourceName { get; set; } = null!;

    public int ProviderId { get; set; }

    public string BaseUrl { get; set; } = null!;

    public string AdditionalUrl { get; set; } = null!;

    public string Endpoint { get; set; } = null!;

    public string Verb { get; set; } = null!;

    public bool IsPlan { get; set; }

    public string ClassText { get; set; } = null!;

    public string ClassName { get; set; } = null!;

    public string FieldFirst { get; set; } = null!;

    public string FieldSecond { get; set; } = null!;

    public string FieldThird { get; set; } = null!;

    public string FieldForth { get; set; } = null!;

    public string FieldFifth { get; set; } = null!;

    public int ResourceType { get; set; }

    public string ResourceParameterDetails { get; set; } = null!;

    public bool IsActive { get; set; }

    public string ResourceResponseParameterDetails { get; set; } = null!;

    public string? ResourceParameterRequestType { get; set; }

    public bool? Lookup { get; set; }

    public virtual Provider Provider { get; set; } = null!;
}
