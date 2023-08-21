using System.ComponentModel.DataAnnotations;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class SignUpVM
    {
        [Key]
        public int UserId { get; set; }
       
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;


        [Required(ErrorMessage = "Mobile Number is required")]
        [Display(Name = "Mobile Number")]
        //[DataType(DataType.PhoneNumber)]
        [StringLength(10, ErrorMessage = "Mobile number must be exactly 10 digits.", MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must consist of 10 digits.")]

        public String MobileNumber { get; set; } = null!;


        [Required(ErrorMessage = "Email is required")]

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Need min 6 characters")]
        public string Password { get; set; } = null!;



       
        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int UpdatedBy { get; set; }


       
    }
}
