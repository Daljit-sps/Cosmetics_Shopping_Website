using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class UserAddress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int StateId { get; set; }

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int PostalCode { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDefault { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual State State { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
