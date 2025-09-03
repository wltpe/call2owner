using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class ErrorCode
{
    public int ErrorCodeId { get; set; }

    public int ProviderId { get; set; }

    public string Code { get; set; } = null!;

    public string ErrorDescription { get; set; } = null!;

    public int Status { get; set; }

    public bool IsActive { get; set; }

    public virtual Provider Provider { get; set; } = null!;
}
