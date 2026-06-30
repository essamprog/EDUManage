using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FinanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}