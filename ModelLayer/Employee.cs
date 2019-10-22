using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataObjects
{
    public class Employee
    {
        #region Data Members

        [Required(ErrorMessage = "Employee ID is required")]
        public int Employee_Id { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [MaxLength(30)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        [MinLength(4)]
        [MaxLength(6)]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Joining Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Joining_Date { get; set; }



        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }


        [Required(ErrorMessage = "Email ID is required")]
        [MaxLength(60)]
        [EmailAddress]
        public string Email { get; set; }



        [Required(ErrorMessage = "Contact no. is required")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Invalid Phone Number")]
        [MinLength(10)]
        [MaxLength(10)]
        public string Contact_Number { get; set; }



        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        public string Password { get; set; }


        [NotMapped] // Does not effect with your database
        [Compare("Password")]
        [MinLength(6)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Organisation ID is required")]
        public string OrganisationId { get; set; }
        
        [Required]
        public string Status { get; set; }

        #endregion
    }
}
