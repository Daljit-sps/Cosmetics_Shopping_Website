using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int CartItemsId { get; set; }

    public int UserAddressId { get; set; }

    public decimal Price { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public virtual CartItem CartItems { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual UserAddress UserAddress { get; set; } = null!;
}
