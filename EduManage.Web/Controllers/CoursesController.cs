using EduManage.Application.DTOs.Courses;
using EduManage.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace EduManage.Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IPhotoService _photoService;
        // المفروض هنا بنحقن كمان الـ ICourseService عشان نحفظ في الداتا بيز

        public CoursesController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        // 1. الدالة دي بتعرض شاشة إضافة الكورس
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2. الدالة دي بتستقبل البيانات من الشاشة لما المستخدم يضغط "حفظ"
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseDto dto, IFormFile imageFile)
        {
            // التأكد إن المستخدم رفع صورة فعلاً
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("", "Please upload the course cover image.");
                return View(dto);
            }

            try
            {
                // 🔥 سطر السحر: رفع الصورة على Cloudinary
                var photoResult = await _photoService.AddPhotoAsync(imageFile);

                // ربط الرابط اللي رجع من كلاوديناري بالـ DTO عشان يتحفظ في الداتا بيز
                dto.ThumbnailUrl = photoResult.Url;

                // لو عامل حقل PublicId في الـ DTO عشان تقدر تمسح الصورة بعدين، ضيف السطر ده:
                // dto.PublicId = photoResult.PublicId; 

                // هنا المفروض بتنادي الـ Service بتاعتك عشان تحفظ الكورس النهائي في الداتا بيز
                // await _courseService.CreateAsync(dto);

                TempData["Success"] = "The course and image have been successfully uploaded!";
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"There was a problem during uploading: {ex.Message}");
                return View(dto);
            }
        }
    }
}