using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class UserAddressVM
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "State")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Pin Code is required")]
        [RegularExpression("[1-9][0-9]{2}[0-9]{3}", ErrorMessage = "Pin Code is not valid")]
        [Display(Name = "Pin Code")]
        public int PostalCode { get; set; }
        
        [Display(Name = "Full Name")]
        public string? FullName { get; set; } 


        [Display(Name = "Mobile Number")]
        public string? MobileNumber { get; set; }

        public string? Email { get; set; }

        public string? State { get; set; }
        public string? Country { get; set; }


        [Display(Name = "Save as Default Address")]
        public bool IsDefault { get; set; }

        public bool IsDelete { get; set; }
    }
}
