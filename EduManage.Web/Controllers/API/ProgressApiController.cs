using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Controllers.API
{
    public class ProgressApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
