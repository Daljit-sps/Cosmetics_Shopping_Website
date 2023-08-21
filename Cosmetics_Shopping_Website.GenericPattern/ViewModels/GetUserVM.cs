using System.ComponentModel.DataAnnotations;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class GetUserVM
    {
        public int UserId { get; set; }
        
       
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

       
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;


    
        [Display(Name = "Mobile Number")]
        [StringLength(10, ErrorMessage = "Mobile number must be exactly 10 digits.", MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must consist of 10 digits.")]

        public String MobileNumber { get; set; } = null!;


        [EmailAddress]
        public string Email { get; set; } = null!;

        
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;



        [Display(Name = "Role Id")]
        public int RoleId { get; set; }

        
        [Display(Name = "State")]
        public int StateId { get; set; }

       
        [Display(Name = "City")]
        public string City { get; set; } = null!;

        
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

       
        [RegularExpression("[1-9][0-9]{2}[0-9]{3}", ErrorMessage = "Pin Code is not valid")]
        [Display(Name = "Pin Code")]
        public int PostalCode { get; set; }

        [Display(Name = "Full Name")]
        public string? FullName { get; set; }


        public string? State { get; set; }
        public string? Country { get; set; }


        [Display(Name = "Save as Default Address")]
        public bool IsDefault { get; set; }


        public bool IsDelete { get; set; }

    }
}
