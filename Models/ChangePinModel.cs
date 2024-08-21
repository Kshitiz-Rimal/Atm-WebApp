using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ATMWebApp.Models
{
    public class ChangePinModel
    {
        [Required(ErrorMessage = "This field cannot be empty")]
        [RegularExpression(@"^1111$", ErrorMessage = "Old Password did not match")]
        public required string OldPin { get; set; }

        [Required(ErrorMessage = "This field cannot be empty")]
        [RegularExpression(@"^(?!1111$)\d{4}$", ErrorMessage = "New Pin must be 4 digit numerical value and cannot be same as old password")]
        public required string NewPin { get; set; }

        public required string AccountNumber { get; set; }
    }
}
