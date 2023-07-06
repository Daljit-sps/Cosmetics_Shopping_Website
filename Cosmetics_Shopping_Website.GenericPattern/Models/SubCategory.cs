using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class SubCategory
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string SubCategoryName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
