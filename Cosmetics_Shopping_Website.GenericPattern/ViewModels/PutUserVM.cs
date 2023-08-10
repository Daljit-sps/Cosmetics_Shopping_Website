using System.ComponentModel.DataAnnotations;

namespace Cosmetics_Shopping_Website.GenericPattern.ViewModels
{
    public class PutUserVM
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;


        [Required(ErrorMessage = "Mobile Number is required")]
        [Display(Name = "Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        public String MobileNumber { get; set; } = null!;


        [Required(ErrorMessage = "Email is required")]

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; } = null!;


        
        /*[DataType(DataType.Password)]
        public string Password { get; set; } = null!;*/


        public DateTime UpdatedOn { get; set; }

        public int UpdatedBy { get; set; }
    }
}
