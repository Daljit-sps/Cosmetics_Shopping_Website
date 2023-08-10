using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class Order_Items_PaymentVM
    {
        [Display(Name ="Order Id")]
        public int OrderId { get; set; }

        public int UserId { get; set; }
        
        [Display(Name ="Name")]
        public string? UserName { get; set; }
        [Display(Name = "Delivery Date")]
        public string? DeliveryDate { get; set; }

        [Display(Name="Order Placed Date")]
        public string? OrderPlacedOn { get; set; }

        [Display(Name ="Payment Status")]
        public string? PaymentStatus { get; set; }

        public decimal? Discount { get; set; }

        [Display(Name ="Total Price")]
        public decimal TotalPrice { get; set; }

        public int OrderItemId { get; set; }

        [Display(Name ="Order Status")]
        public string? OrderStatus { get; set; }

        public decimal Price { get; set; }

        public int ProductVariantId { get; set; }

        public string? ProductVariantName { get; set; }

        public string? VariantImage { get; set; }

        public int Quantity { get; set; }
        public int UserAddressId { get; set; }

        [Display(Name ="Delivery Address")]
        public string? UserAddress { get; set; }

        public string? City { get; set; }

        public bool IsDelete { get; set; }
    }
}
