using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class CartItemsVM
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public string VariantName { get; set; } = null!;

        public string VariantDescription { get; set; } = null!;

        public decimal Price { get; set; }

        public string? VariantImage { get; set; }

        public decimal CartItemPrice { get; set; }

      
    }
}
