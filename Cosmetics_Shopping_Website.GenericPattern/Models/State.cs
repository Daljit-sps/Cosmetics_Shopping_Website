using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class State
{
    public int Id { get; set; }

    public string StateName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public int CountryId { get; set; }

    public int UserId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    public virtual ICollection<VariantsAvailability> VariantsAvailabilities { get; set; } = new List<VariantsAvailability>();
}
