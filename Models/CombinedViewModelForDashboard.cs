namespace ATMWebApp.Models
{
    public class CombinedViewModelForDashboard
    {
        public required List<User> Users { get; set; }

        public required ATMWebApp.Models.User User { get; set;}

        public required AddUserDataModel AddUserData { get; set; }
    }
}
