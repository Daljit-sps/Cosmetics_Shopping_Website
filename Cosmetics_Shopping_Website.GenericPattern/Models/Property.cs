using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class Property
{
    public int Id { get; set; }

    public string PropertyName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<ProductVariantsProperty> ProductVariantsProperties { get; set; } = new List<ProductVariantsProperty>();
}
