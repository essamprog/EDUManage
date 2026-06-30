using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Services
{
    public class NotificationService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
