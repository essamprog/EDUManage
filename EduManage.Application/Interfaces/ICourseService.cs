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
        Task<IEnumerable<CourseDto>> GetPublishedCoursesAsync();
        Task<IEnumerable<CourseDto>> GetByInstructorAsync(int instructorId);
        
        // Step 1: Create Draft
        Task<CourseDto> CreateAsync(int instructorId, CreateCourseDto dto);
        
        // Step 2: Curriculum
        Task<CourseSectionDto> AddSectionAsync(int courseId, string title);
        Task<CourseLessonDto> AddLessonAsync(int sectionId, CreateLessonDto dto);
        
        // Step 3: Media & Update
        Task<CourseDto> UpdateMediaAsync(int courseId, string? thumbnailUrl, string? thumbnailKey, string? promoUrl, string? promoKey);
        Task<CourseDto> UpdateAsync(int id, UpdateCourseDto dto);
        
        Task<bool> PublishAsync(int id);
        Task DeleteAsync(int id);
    }
}