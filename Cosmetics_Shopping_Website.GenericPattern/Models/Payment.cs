using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string PaymentType { get; set; } = null!;

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public virtual Order Order { get; set; } = null!;
}
