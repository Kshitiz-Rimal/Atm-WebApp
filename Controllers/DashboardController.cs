using ATMWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Buffers;
using System.Diagnostics;
using System.Reflection;

namespace ATMWebApp.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult UserDashboard()
        {
            if (TempData["User"] != null)
            {
                string? currentUserJson = TempData["User"].ToString();

                if (currentUserJson != null)
                {
                    User? currentUser = JsonConvert.DeserializeObject<User>(currentUserJson);
                    ViewBag.User = currentUser;
                    TempData["User"] = JsonConvert.SerializeObject(currentUser);
                }

            }
            return View();
        }

        [HttpPost]
        public IActionResult WithdrawMoney(string withdrawAmount, string accountNumber)
        {
            if (ModelState.IsValid)
            {
                string validWithdrawAmountResponse = ValidateWithdrawAmount(withdrawAmount, accountNumber);
                if (validWithdrawAmountResponse == "success")
                {
                    return Json(new { success = true, message = "Money withdrawn successfully" });
                }
                else
                {
                    return Json(new { success = false, message = validWithdrawAmountResponse });
                }
            }
            else
            {
                return Json(new { success = false, message = "Bad Response" });
            }
        }

        [HttpGet]
        public IActionResult AdminDashboard()
        {
            if (TempData["User"] != null)
            {
                string? currentUserJson = TempData["User"].ToString();
                if (currentUserJson != null)
                {
                    User? currentUser = JsonConvert.DeserializeObject<User>(currentUserJson);

                    TempData["User"] = JsonConvert.SerializeObject(currentUser);
                    List<User> allUsers = GetAllUsers();
                    var viewModel = new CombinedViewModelForDashboard
                    {
                        Users = allUsers,
                        User = currentUser,
                        AddUserData = null
                    };
                    ViewBag.User = viewModel;
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult AdminDashboard(string searchValue)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                string? currentUserJson = TempData["User"].ToString();
                if (currentUserJson != null)
                {
                    User? currentUser = JsonConvert.DeserializeObject<User>(currentUserJson);

                    TempData["User"] = JsonConvert.SerializeObject(currentUser);

                    List<User>? searchedUsers = GetSearchResults(searchValue);

                    var viewModel = new CombinedViewModelForDashboard
                    {
                        Users = searchedUsers,
                        User = currentUser,
                        AddUserData = null
                    };
                    ViewBag.User = viewModel;
                }
                return View();
            }
            else
            {
                string? currentUserJson = TempData["User"].ToString();
                if (currentUserJson != null)
                {
                    User? currentUser = JsonConvert.DeserializeObject<User>(currentUserJson);

                    TempData["User"] = JsonConvert.SerializeObject(currentUser);

                    List<User> allUsers = GetAllUsers();
                    var viewModel = new CombinedViewModelForDashboard
                    {
                        Users = allUsers,
                        User = currentUser,
                        AddUserData = null
                    };
                    ViewBag.User = viewModel;
                }
                return View();
            }
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] AddUserDataModel addUserDataModel)
        {
            if (ModelState.IsValid && TempData["User"] != null)
            {
                string? currentUserJson = TempData["User"].ToString();
                if (ValidateAccountNUmber(addUserDataModel))
                {
                    SaveNewCostumer(addUserDataModel);
                    List<User> allUsers = GetAllUsers();
                    User? currentUser = JsonConvert.DeserializeObject<User>(currentUserJson);

                    TempData["User"] = JsonConvert.SerializeObject(currentUser);

                    var viewModel = new CombinedViewModelForDashboard
                    {
                        Users = allUsers,
                        User = currentUser,
                        AddUserData = null
                    };
                    ViewBag.User = viewModel;
                    return Json(new { success = true });
                }
                else
                {
                    User? currentUser = JsonConvert.DeserializeObject<User>(currentUserJson);
                    TempData["User"] = JsonConvert.SerializeObject(currentUser);
                    return Json(new { success = false, message = "Account Number Already exists" });
                }
            }
            else
            {
                var errorMessages = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

                // Return a JSON response with success set to false and error messages
                return Json(new { success = false, message = "Validation errors occurred", errors = errorMessages });
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePinModel changePinModel)
        {
            if (ModelState.IsValid)
            {
                UserData userData = new CRUDUserData();
                List<User> allUsers = userData.LoadUserData("wwwroot/Json/UserDetails.json");
                User oldUser = allUsers.Find(u => u.AccountNumber == changePinModel.AccountNumber);
                allUsers.Remove(oldUser);
                User user = new(oldUser.UserName, oldUser.AccountNumber, oldUser.Balance, oldUser.AccountStatus, changePinModel.NewPin, oldUser.AccountType, "false");
                allUsers.Add(user);
                userData.SaveUserData(allUsers, "wwwroot/Json/UserDetails.json");
                var userDataJson = JsonConvert.SerializeObject(user);
                TempData["User"] = userDataJson;
                return RedirectToAction("UserDashboard", "Dashboard");
            }
            else
            {
                return PartialView("_firstLoginPartialView");
            }
        }

        public IActionResult EditUserView(string accountNumber)
        {
            if (ModelState.IsValid)
            {
                List<User> thisUser = GetSearchResults(accountNumber);
                User currentUser = thisUser[0];
                ViewBag.User = currentUser;
            }
            return View();
        }

        [HttpPost]
        public IActionResult EditUserData(EditUserModel editUserModel)
        {
            if (ModelState.IsValid)
            {
                SaveEditedCustomer(editUserModel);
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            else
            {
                return View();
            }
        }

        public IActionResult DeleteUser(string accountNumber)
        {
            if (ModelState.IsValid)
            {
                UserData userData = new CRUDUserData();
                List<User> allUsers = userData.LoadUserData("wwwroot/Json/UserDetails.json");
                User userToBeDeleted = allUsers.Find(u => u.AccountNumber == accountNumber);
                allUsers.Remove(userToBeDeleted);
                userData.SaveUserData(allUsers, "wwwroot/Json/UserDetails.json");
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            else
            {
                return View();
            }
        }

        private static List<User> GetAllUsers()
        {
            UserData userData = new CRUDUserData();
            List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");
            List<User> newUserList = [];
            foreach (var user in users)
            {
                if (user.AccountType == "Customer")
                {
                    newUserList.Add(user);
                }

            }
            return newUserList;
        }

        private static List<User> GetSearchResults(string searchValue)
        {
            UserData userData = new CRUDUserData();
            List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");
            List<User> newUserList = [];
            foreach (var user in users)
            {
                if ((user.AccountType == "Customer" && user.UserName == searchValue) || (user.AccountType == "Customer" && user.AccountNumber == searchValue))
                {
                    newUserList.Add(user);
                }
            }
            return newUserList;
        }

        private static bool ValidateAccountNUmber(AddUserDataModel addUserDataModel)
        {
            UserData userData = new CRUDUserData();
            List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");
            User? user = users.Find(u => u.AccountNumber == addUserDataModel.AccountNumber);

            if (user != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void SaveNewCostumer(AddUserDataModel addUserDataModel)
        {
            UserData userData = new CRUDUserData();
            User user = new(addUserDataModel.CustomerName, addUserDataModel.AccountNumber, double.Parse(addUserDataModel.Balance), "Activated", "1111", "Customer", "true");
            userData.AddUser(user, "wwwroot/Json/UserDetails.json");

        }

        private static void SaveEditedCustomer(EditUserModel editUserModel)
        {
            UserData userData = new CRUDUserData();
            List<User> allUsers = userData.LoadUserData("wwwroot/Json/UserDetails.json");
            User oldUser = allUsers.Find(u => u.AccountNumber == editUserModel.AccountNumber);
            allUsers.Remove(oldUser);
            double newBalance = oldUser.Balance + editUserModel.Balance;
            User user = new(editUserModel.UserName, editUserModel.AccountNumber, newBalance, editUserModel.AccountStatus, editUserModel.Pin, oldUser.AccountType, oldUser.FirstLogin);
            allUsers.Add(user);
            userData.SaveUserData(allUsers, "wwwroot/Json/UserDetails.json");
        }

        private string ValidateWithdrawAmount(string withdrawAmount, string accountNumber)
        {
            UserData userData = new CRUDUserData();
            List<User> allUsers = userData.LoadUserData("wwwroot/Json/UserDetails.json");
            User currentUser = allUsers.Find(u => u.AccountNumber == accountNumber);
            allUsers.Remove(currentUser);
            if (currentUser.Balance > double.Parse(withdrawAmount) && currentUser.Balance - double.Parse(withdrawAmount) >= 1000)
            {
                double newBalance = currentUser.Balance - double.Parse(withdrawAmount);
                User user = new(currentUser.UserName, currentUser.AccountNumber, newBalance, currentUser.AccountStatus, currentUser.Password, currentUser.AccountType, currentUser.FirstLogin);
                allUsers.Add(user);
                userData.SaveUserData(allUsers, "wwwroot/Json/UserDetails.json");
                TempData["User"] = JsonConvert.SerializeObject(user);
                return "success";
            }
            else if (currentUser.Balance > double.Parse(withdrawAmount) && currentUser.Balance - double.Parse(withdrawAmount) < 1000)
            {
                return "User needs minimum balance of 1000";
            }
            else
            {
                return "Insufficient Balance";
            }

        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
