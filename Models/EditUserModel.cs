using System.ComponentModel.DataAnnotations;

namespace ATMWebApp.Models
{
    public class EditUserModel
    {
        [RegularExpression(@"^[A-Z][a-z]+(?: [A-Z][a-z]+)*(?:-[A-Z][a-z]+)*$", ErrorMessage = "Invalid Costumer Name")]
        public required string UserName { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "New PIN must be exactly 4 numerical digits")]
        public required string Pin { get; set; }

        public required string AccountNumber { get; set; }

        public required string AccountStatus { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Balance to be added must be numerical value")]
        public required double Balance { get; set; }

    }
}
