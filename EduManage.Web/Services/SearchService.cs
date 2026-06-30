using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Services
{
    public class SearchService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
