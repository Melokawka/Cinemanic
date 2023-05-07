using cinemanic.Models;
using Microsoft.AspNetCore.Mvc;

namespace cinemanic.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Authenticate user
                // TODO: Create authentication cookie and redirect to dashboard
            }

            return View(model);
        }
    }
}
