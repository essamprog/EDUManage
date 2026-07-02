using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EduManage.Application.Interfaces
{
    public interface IPhotoService
    {
        // دالة لرفع الصورة الافتراضية
        Task<(string Url, string PublicId)> AddPhotoAsync(IFormFile file);

        // دالة لرفع صورة في مجلد معين بدون قص (للـ Thumbnails)
        Task<(string Url, string PublicId)> AddImageAsync(IFormFile file, string folderPath);

        // دالة لرفع فيديو، هترجع الرابط، الـ PublicId، والمدة بالثواني
        Task<(string Url, string PublicId, int Duration)> AddVideoAsync(IFormFile file, string folderPath);

        // دالة لمسح الصورة أو الفيديو من كلاوديناري
        Task<bool> DeletePhotoAsync(string publicId);
        Task<bool> DeleteVideoAsync(string publicId);
    }
}