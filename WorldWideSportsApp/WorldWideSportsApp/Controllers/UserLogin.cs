using Microsoft.AspNetCore.Mvc;

namespace WorldWideSportsApp.Controllers
{
    public class UserLogin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
