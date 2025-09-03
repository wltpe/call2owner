using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Setting
{
    public int SettingId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string SettingValue { get; set; } = null!;

    public bool IsActive { get; set; }
}
