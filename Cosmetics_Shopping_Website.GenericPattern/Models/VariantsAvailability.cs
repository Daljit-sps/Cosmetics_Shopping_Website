using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class VariantsAvailability
{
    public int Id { get; set; }

    public int StateId { get; set; }

    public int ProductVariantId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ProductVariant ProductVariant { get; set; } = null!;

    public virtual State State { get; set; } = null!;
}
