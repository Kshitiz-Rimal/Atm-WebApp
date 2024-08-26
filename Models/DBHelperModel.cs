using System;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using MySql.Data.MySqlClient;

namespace ATMWebApp.Models
{
    public class DBHelperModel
    {
        private readonly string connectionString;

        // Constructor to initialize the connection string
        public DBHelperModel(string server, string database, string userId, string password)
        {
            connectionString = $"Server={server};Database={database};User ID={userId};Password={password};SslMode=none;";
        }

        // Method to open the connection
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Method to fetch all records from the employee_details table
        public MySqlDataReader FetchEmployeeDetails()
        {
            var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM employee_details ORDER BY AccountNumber;";
            var cmd = new MySqlCommand(query, conn);

            return cmd.ExecuteReader(); // Caller is responsible for closing the reader and connection
        }
        
        // Method to Search user based on their name or account number
        public MySqlDataReader SearchUser(string searchValue)
        {
            var conn = GetConnection();
            conn.Open();

            string query = $"SELECT * FROM employee_details WHERE REPLACE(TRIM(AccountNumber), ' ', '') LIKE CONCAT('%', '{searchValue}', '%') OR LOWER(REPLACE(TRIM(UserName), ' ', '')) LIKE LOWER(CONCAT('%', '{searchValue}', '%')) ORDER BY AccountNumber;";
            var cmd = new MySqlCommand(query, conn);

            return cmd.ExecuteReader();
        }

        public void DeleteUser(string accountNumber) 
        {
            string query = "DELETE FROM employee_details WHERE AccountNumber = @AccountNumber";

            using MySqlConnection conn = new(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User deleted successfully!");
                }
                else
                {
                    Console.WriteLine("No user found with the specified account number.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void UpdateUserDetails(string accountNumber, string userName, double balanceToAdd, string password, string accountStatus, string firstLogin)
        {
            string query = "UPDATE employee_details " +
                  "SET UserName = @UserName, " +
                  "    Balance = Balance + @BalanceToAdd, " + // Add the specified amount to the current balance
                  "    Password = @Password, " +
                  "    AccountStatus = @AccountStatus, " +
                  "    FirstLogin = @FirstLogin " +
                  "WHERE AccountNumber = @AccountNumber";

            using MySqlConnection conn = new(connectionString);
            try
            {
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                // Adding parameters with proper types
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@BalanceToAdd", balanceToAdd); // The amount to add to the current balance
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@AccountStatus", accountStatus);
                cmd.Parameters.AddWithValue("@FirstLogin", firstLogin);
                cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User details updated successfully!");
                }
                else
                {
                    Console.WriteLine("No user found with the specified account number.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void AddNewUser(string userName, string accountNumber, string balance)
        {
            
            string query = $"INSERT INTO employee_details (UserName, AccountNumber, Balance, AccountStatus, Password, AccountType, FirstLogin) VALUES (@UserName, @AccountNumber, @Balance, @AccountStatus, @Password, @AccountType, @FirstLogin)";

            using (MySqlConnection conn = new(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@UserName", $"{userName}");
                    cmd.Parameters.AddWithValue("@AccountNumber", $"{accountNumber}");
                    cmd.Parameters.AddWithValue("@Balance", $"{double.Parse(balance)}");
                    cmd.Parameters.AddWithValue("@AccountStatus", $"Activated");
                    cmd.Parameters.AddWithValue("@Password", $"1111");
                    cmd.Parameters.AddWithValue("@AccountType", $"Customer");
                    cmd.Parameters.AddWithValue("@FirstLogin", $"true");

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("User added successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to add the user.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

    }
}
        