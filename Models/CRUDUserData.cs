
using System.Text.Json;

namespace ATMWebApp.Models
{
    internal class CRUDUserData : UserData
    {
        public override void AddUser(User user, string filePath)
        {
            List<User> users = LoadUserData(filePath);
            users.Add(user);
            users.Sort((a, b) =>
            {
                if (a.AccountNumber == null && b.AccountNumber == null) return 0;
                if (a.AccountNumber == null) return 1;
                if (b.AccountNumber == null) return -1;
                return double.Parse(a.AccountNumber).CompareTo(double.Parse(b.AccountNumber));
            });
            var json = JsonSerializer.Serialize(users);
            File.WriteAllText(filePath, json);
        }

        public override void DeleteUserData(User user, string filePath)
        {
            List<User> usersWithAdmin = LoadUserData(filePath);

            User? userToRemove = usersWithAdmin.Find(x => x.AccountNumber == user.AccountNumber);
            usersWithAdmin.Remove(userToRemove);
            SaveUserData(usersWithAdmin, filePath);
        }

        public override List<User> LoadUserData(string filePath)
        {
            using (StreamReader file = File.OpenText(filePath))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<User>? users = JsonSerializer.Deserialize<List<User>>(file.ReadToEnd(), options).ConvertAll(u => (User)u);
                //users.Sort((x, y) => double.Parse(x.AccountNumber).CompareTo(double.Parse(y.AccountNumber)));
                users.Sort((a, b) =>
                {
                    if (a.AccountNumber == null && b.AccountNumber == null) return 0;
                    if (a.AccountNumber == null) return 1;
                    if (b.AccountNumber == null) return -1;
                    return double.Parse(a.AccountNumber).CompareTo(double.Parse(b.AccountNumber));
                });
                return users;
            }
        }

        public override void SaveUserData(List<User> users, string filePath)
        {
            // Serialize the list of User objects to JSON and write to file
            users.Sort((a, b) =>
            {
                if (a.AccountNumber == null && b.AccountNumber == null) return 0;
                if (a.AccountNumber == null) return 1;
                if (b.AccountNumber == null) return -1;
                return double.Parse(a.AccountNumber).CompareTo(double.Parse(b.AccountNumber));
            });
            var json = JsonSerializer.Serialize(users);
            File.WriteAllText(filePath, json);
        }
    }
}
