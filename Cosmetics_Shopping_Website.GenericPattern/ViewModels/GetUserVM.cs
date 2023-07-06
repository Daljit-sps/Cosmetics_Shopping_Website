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
        public String MobileNumber { get; set; } = null!;


        [EmailAddress]
        public string Email { get; set; } = null!;

        
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;



        [Display(Name = "Role Id")]
        public int RoleId { get; set; }

        public bool IsDelete { get; set; }

    }
}
