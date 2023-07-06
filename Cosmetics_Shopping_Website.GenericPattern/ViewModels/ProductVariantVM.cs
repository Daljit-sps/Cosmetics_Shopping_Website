using Cosmetics_Shopping_Website.GenericPattern.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class ProductVariantVM
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Product Name is required")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Variant Name is required")]
        [Display(Name = "Variant")]
        public string VariantName { get; set; } = null!;

        [Required(ErrorMessage = "Variant Description is required")]
        [Display(Name = "Variant Description")]
        public string VariantDescription { get; set; } = null!;

        [Required(ErrorMessage = "Variant Image is required")]
        [Display(Name = "Variant Image")]
        public IFormFile? ImageFile { get; set; } 

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        public string? Product { get; set; } 
        public string? VariantImage { get; set; } 

        //public int WishlistId { get; set; }
        public bool IsDelete { get; set; }

        public bool IsWishlisted { get; set; }
    }
}
