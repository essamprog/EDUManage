using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    public class FinancialsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}