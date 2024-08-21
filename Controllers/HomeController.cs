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
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
                User? userData = UserAccountLogin(formData);

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
                User? userData = AdminAccountLogin(formData);

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

        private static User? UserAccountLogin(FormData formData)
        {
            UserData userData = new CRUDUserData();
            List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");

            User? user = users.Find(u => u.AccountNumber == formData.UName);
            if (user != null)
            {
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
                return null;
            }
        }

        private static User? AdminAccountLogin(FormData formData)
        {
            UserData userData = new CRUDUserData();
            List<User> users = userData.LoadUserData("wwwroot/Json/UserDetails.json");

            User? user = users.Find(u => u.UserName == formData.UName);
            if (user != null)
            {
                if (user.Password == formData.Password)
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
                return null;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
