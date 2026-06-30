using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class EnrollmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}