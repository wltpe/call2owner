using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Config
{
    public int ConfigId { get; set; }

    public int ProviderId { get; set; }

    public string ConfigKey { get; set; } = null!;

    public string ConfigValue { get; set; } = null!;

    public bool IsPlan { get; set; }

    public bool IsActive { get; set; }
}
