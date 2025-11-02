using Microsoft.AspNetCore.Mvc;

namespace PortfoyApi.Controllers
{
    public class PortfoyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
