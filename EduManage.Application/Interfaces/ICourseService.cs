using EduManage.Application.DTOs.Common;
using EduManage.Application.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduManage.Application.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResult<CourseDto>> GetAllAsync(CourseFilterDto filter);
        Task<CourseDto?> GetByIdAsync(int id);
        Task<CourseDto> CreateAsync(int instructorId, CreateCourseDto dto);
        Task<CourseDto> UpdateAsync(int id, UpdateCourseDto dto);
        Task DeleteAsync(int id);
        Task<bool> PublishAsync(int id);
        Task<IEnumerable<CourseDto>> GetByInstructorAsync(int instructorId);

        Task<IEnumerable<CourseDto>> GetPublishedCoursesAsync();
    }
}