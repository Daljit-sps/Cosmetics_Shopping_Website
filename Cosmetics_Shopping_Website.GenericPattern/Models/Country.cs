using System;
using System.Collections.Generic;

namespace Cosmetics_Shopping_Website.GenericPattern.Models;

public partial class Country
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CountryName { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();

    public virtual User User { get; set; } = null!;
}
