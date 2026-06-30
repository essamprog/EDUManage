using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Services
{
    public class LessonProgressService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
