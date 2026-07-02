using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Area.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        public IActionResult Index() => View();
    }
}
