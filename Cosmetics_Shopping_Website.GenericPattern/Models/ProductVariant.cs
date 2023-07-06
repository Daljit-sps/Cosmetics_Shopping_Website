using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string VariantName { get; set; } = null!;

    public string VariantDescription { get; set; } = null!;

    public string VariantImage { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Product Product { get; set; } = null!;
    

    public virtual ICollection<ProductVariantsProperty> ProductVariantsProperties { get; set; } = new List<ProductVariantsProperty>();

    public virtual ICollection<VariantsAvailability> VariantsAvailabilities { get; set; } = new List<VariantsAvailability>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
