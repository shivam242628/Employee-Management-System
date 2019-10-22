using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataObjects
{
    public class Admin
    {
        #region Data Members
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30)]
        public string Admin_User_Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        public string Password { get; set; }


        [NotMapped] 
        [Compare("Password")]
        [Required(ErrorMessage = "Re-enter the same password.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Organisation ID is required")]
        public string OrganisationID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "Organisation Address is required")]
        public string OrganisationAddress { get; set; }

        [Required(ErrorMessage = "Email ID is required")]
        [MaxLength(60)]
        [EmailAddress]
        public string AdminEmail { get; set; }

        #endregion
    }
}
