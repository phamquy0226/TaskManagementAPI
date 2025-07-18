using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class NoteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
