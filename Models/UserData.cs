namespace ATMWebApp.Models
{
    abstract class UserData
    {
        public abstract List<User> LoadUserData(string filePath);

        public abstract void SaveUserData(List<User> users, string filePath);

        public abstract void DeleteUserData(User user, string filePath);

        public abstract void AddUser(User user, string filePath);
    }
}
