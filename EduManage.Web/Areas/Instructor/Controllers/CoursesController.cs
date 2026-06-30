using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(object model)
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(int id, object model)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}