using Microsoft.AspNetCore.Mvc;

namespace WorldWideSportsApp.Controllers
{
    public class StatsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
