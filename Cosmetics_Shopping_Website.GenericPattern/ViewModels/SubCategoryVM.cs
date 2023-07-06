using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class SubCategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Sub Category Name is required")]
        [Display(Name = "Sub Category")]
        public string SubCategoryName { get; set; } = null!;

        
        public string? Category { get; set; }

        public bool IsDelete { get; set; }

    }
}
