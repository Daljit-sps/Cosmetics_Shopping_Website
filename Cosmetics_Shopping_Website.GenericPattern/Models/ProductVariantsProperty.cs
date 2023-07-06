using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class ProductVariantsProperty
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int ProductVariantId { get; set; }

    public string Value { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public virtual ProductVariant ProductVariant { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;
}
