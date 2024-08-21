using System.ComponentModel.DataAnnotations;

namespace ATMWebApp.Models
{
    public class FormData
    {
        [Required(ErrorMessage ="This field cannot be empty")]
        public required string UName { get; set; }

        [Required(ErrorMessage = "This Field Cannot be empty")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 numerical digits")]
        public required string Pin { get; set; }

        [Required(ErrorMessage = "This Field Cannot be empty")]
        public required string Password { get; set; }
    }
}
