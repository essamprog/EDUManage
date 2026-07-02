using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EduManage.Application.DTOs.System;
using EduManage.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EduManage.Application.Services.System
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<(string Url, string PublicId)> AddPhotoAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "EduManage-Courses"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null) throw new Exception(uploadResult.Error.Message);

                return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
            }
            throw new Exception("File is empty");
        }

        public async Task<(string Url, string PublicId)> AddImageAsync(IFormFile file, string folderPath)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folderPath
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null) throw new Exception(uploadResult.Error.Message);

                return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
            }
            throw new Exception("File is empty");
        }

        public async Task<(string Url, string PublicId, int Duration)> AddVideoAsync(IFormFile file, string folderPath)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folderPath
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null) throw new Exception(uploadResult.Error.Message);

                int duration = 0;
                if (uploadResult.Duration > 0)
                {
                    duration = (int)Math.Round(uploadResult.Duration);
                }

                return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId, duration);
            }
            throw new Exception("File is empty");
        }

        public async Task<bool> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        public async Task<bool> DeleteVideoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId) { ResourceType = ResourceType.Video };
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
    }
}