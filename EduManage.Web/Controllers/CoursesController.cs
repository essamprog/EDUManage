using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Browse()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}