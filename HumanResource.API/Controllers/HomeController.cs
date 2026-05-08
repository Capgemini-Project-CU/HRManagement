using Microsoft.AspNetCore.Mvc;

namespace HumanResource.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
