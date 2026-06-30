using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Controllers.API
{
    public class NotificationsApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
