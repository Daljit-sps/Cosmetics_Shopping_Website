using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class VariantAvailabilityVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "State Name is required")]
        [Display(Name = "States")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "Product-Variant Name is required")]
        [Display(Name = "Product Variant")]
        public int ProductVariantId { get; set; }

        [Display(Name = "Product-Variant")]
        public string? ProductVariantName { get; set; }

        public string? State { get; set; }
        [Required(ErrorMessage = "Availability Check is required")]
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }

        public bool IsDelete { get; set; }
    }
}
