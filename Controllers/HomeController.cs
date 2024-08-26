using ATMWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace ATMWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBHelperModel _dbHelper;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DBHelperModel dbHelper)
        {
            _logger = logger;
            _dbHelper = dbHelper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(FormData formData)
        {
            if (ModelState.IsValid)
            {
                User? userData = AccountLogin(formData, "Customer");

                if (userData != null && userData.AccountStatus == "Activated")
                {
                    var userDataJson = JsonConvert.SerializeObject(userData);
                    TempData["User"] = userDataJson;
                    return RedirectToAction("UserDashboard", "Dashboard");
                }
                else if (userData != null && userData.AccountStatus != "Activated")
                {
                    ModelState.AddModelError(string.Empty, "Your account has been deactivated!!!! Please Consult your bank.");
                    return View(formData);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Account number or pin.");
                    return View(formData);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Account number or pin.");
                return View(formData);
            }
        }

        [HttpPost]
        public IActionResult AdminLogin(FormData formData)
        {
            if (ModelState.IsValid)
            {
                User? userData = AccountLogin(formData, "Admin");

                if (userData != null)
                {
                    var userDataJson = JsonConvert.SerializeObject(userData);
                    TempData["User"] = userDataJson;
                    return RedirectToAction("AdminDashboard", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Admin Name or password.");
                    return View(formData);
                }
            }
            else
            {
                //ModelState.AddModelError(string.Empty, "Invalid Admin Name or password.");
                return View(formData);
            }
        }

        private User? AccountLogin(FormData formData, string accountType)
        {
            //UserData userData = new CRUDUserData();
            //List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");
            //User? user = users.Find(u => u.AccountNumber == formData.UName);
            List<User> usersList = GetSearchResults(formData.UName);

            if (usersList != null && accountType == "Customer")
            {
                User? user = usersList.FirstOrDefault(u => u.AccountType.Equals("Admin", StringComparison.OrdinalIgnoreCase));
                if (user.Password == formData.Pin)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                User? user = usersList.FirstOrDefault(u => u.AccountType.Equals("Admin", StringComparison.OrdinalIgnoreCase));
                if (user != null && user.Password == formData.Password)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        //private static User? AdminAccountLogin(FormData formData)
        //{
        //    UserData userData = new CRUDUserData();
        //    List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");

        //    User? user = users.Find(u => u.UserName == formData.UName);
        //    if (user != null)
        //    {
        //        if (user.Password == formData.Password)
        //        {
        //            return user;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        private List<User> GetSearchResults(string searchValue)
        {
            //UserData userData = new CRUDUserData();
            //List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");

            List<User> users = [];
            try
            {
                using var reader = _dbHelper.SearchUser(searchValue);
                while (reader.Read())
                {
                    users.Add(new User(
                        userName: reader["UserName"].ToString(),
                        accountNumber: reader["AccountNumber"].ToString(),
                        balance: Convert.ToDouble(reader["Balance"]),
                        accountStatus: reader["AccountStatus"].ToString(),
                        password: reader["Password"].ToString(),
                        accountType: reader["AccountType"].ToString(),
                        firstLogin: reader["FirstLogin"].ToString()
                    ));
                }
            }
            catch (Exception ex)
            {
                // Handle exception, e.g., log it or return an error view
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            //List<User> newUserList = [];
            //foreach (var user in users)
            //{
            //    if ((user.AccountType == "Customer" && user.UserName == searchValue) || (user.AccountType == "Customer" && user.AccountNumber == searchValue))
            //    {
            //        newUserList.Add(user);
            //    }
            //}
            return users;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
