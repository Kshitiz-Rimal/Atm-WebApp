using System.ComponentModel.DataAnnotations;

namespace ATMWebApp.Models
{
    public class AddUserDataModel
    {
        [Required(ErrorMessage = "This field cannot be empty")]
        [RegularExpression(@"^[A-Z][a-z]+(?: [A-Z][a-z]+)*(?:-[A-Z][a-z]+)*$", ErrorMessage = "Invalid Costumer Name")]
        public required string CustomerName { get; set; }

        [Required(ErrorMessage = "This Field Cannot be empty")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "PIN must be exactly 10 numerical digits")]
        public required string AccountNumber { get; set; }

        [Required(ErrorMessage = "This Field Cannot be empty")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Balance must be numerical value")]
        public required string Balance { get; set; }


    }
}
