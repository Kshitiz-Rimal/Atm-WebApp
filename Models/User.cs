namespace ATMWebApp.Models
{
    public class User(string userName, string accountNumber, double balance, string accountStatus, string password, string accountType, string firstLogin)
    {
        public string UserName { get; set; } = userName;

        public string AccountNumber { get; set; } = accountNumber;

        public double Balance { get; set; } = balance;

        public string AccountStatus { get; set; } = accountStatus;

        public string Password { get; set; } = password;

        public string AccountType { get; set; } = accountType;

        public string FirstLogin { get; set; } = firstLogin;
    }
}
