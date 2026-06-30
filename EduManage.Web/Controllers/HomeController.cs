using EduManage.Application.Interfaces;
using EduManage.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EduManage.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;

        // عملنا حقن (Injection) لخدمة الكورسات هنا
        public HomeController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            // بنسحب كل الكورسات من الداتا بيز
            // (لو مسمين الدالة اسم تاني في الـ Interface عندكم، عدلها للاسم الصح)
            var courses = await _courseService.GetPublishedCoursesAsync();

            // بنبعت الكورسات دي لصفحة الـ HTML عشان تعرضها
            return View(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}