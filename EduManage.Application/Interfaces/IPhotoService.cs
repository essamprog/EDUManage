using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EduManage.Application.Interfaces
{
    public interface IPhotoService
    {
        // دالة لرفع الصورة، هترجع رابط الصورة (URL) والـ PublicId عشان لو حابين نمسحها بعدين
        Task<(string Url, string PublicId)> AddPhotoAsync(IFormFile file);

        // دالة لمسح الصورة من كلاوديناري
        Task<bool> DeletePhotoAsync(string publicId);
    }
}