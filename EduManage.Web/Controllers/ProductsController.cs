using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
         {
            return View();
        }
    }
}
