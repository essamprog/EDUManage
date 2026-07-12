// Application/Services/Courses/CourseService.cs
using AutoMapper;
using EduManage.Application.DTOs.Common;
using EduManage.Application.DTOs.Courses;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduManage.Application.Services.Courses;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CourseService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<CourseDto>> GetAllAsync(CourseFilterDto filter)
    {
        var query = _uow.Courses.Query()
            .Include(c => c.Instructor).ThenInclude(i => i.User)
            .Where(c =>
                c.Status == CourseStatus.Published &&
                (filter.InstructorId == null || c.InstructorId == filter.InstructorId) &&
                (filter.Search == null || c.Title.Contains(filter.Search)) &&
                (filter.CategoryId == null || c.CategoryId == filter.CategoryId) &&
                (filter.Level == null || c.Level == filter.Level) &&
                (filter.MinPrice == null || c.Price >= filter.MinPrice) &&
                (filter.MaxPrice == null || c.Price <= filter.MaxPrice));

        query = filter.SortBy switch
        {
            "popular" => query.OrderByDescending(c => c.TotalStudents),
            "rating"  => query.OrderByDescending(c => c.AverageRating),
            "price"   => query.OrderBy(c => c.Price),
            _         => query.OrderByDescending(c => c.CreatedAt),
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<CourseDto>
        {
            Items = _mapper.Map<IEnumerable<CourseDto>>(items),
            Total = total,
            Page = filter.Page,
            PageSize = filter.PageSize,
        };
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var course = await _uow.Courses.GetByIdAsync(id);
        if (course != null)
        {
            // Eager load sections and lessons manually for DTO mapping
            course.Sections = (await _uow.CourseSections.FindAsync(s => s.CourseId == id)).OrderBy(s => s.OrderIndex).ToList();
            foreach (var section in course.Sections)
            {
                section.Lessons = (await _uow.CourseLessons.FindAsync(l => l.SectionId == section.Id)).OrderBy(l => l.OrderIndex).ToList();
            }
        }
        return course is null ? null : _mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> CreateAsync(int instructorId, CreateCourseDto dto)
    {
        var course = _mapper.Map<Course>(dto);
        course.InstructorId = instructorId;
        course.Status = CourseStatus.Draft;

        await _uow.Courses.AddAsync(course);
        await _uow.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var course = await _uow.Courses.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("The course does not exist");

        _mapper.Map(dto, course);
        course.UpdatedAt = DateTime.UtcNow;

        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _uow.Courses.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("The course does not exist");

        course.DeletedAt = DateTime.UtcNow;   // Soft Delete
        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();
    }

    public async Task<bool> PublishAsync(int id)
    {
        var course = await _uow.Courses.GetByIdAsync(id);
        if (course is null) return false;

        course.Status = CourseStatus.Published;
        course.PublishedAt = DateTime.UtcNow;
        course.UpdatedAt = DateTime.UtcNow;

        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CourseDto>> GetPublishedCoursesAsync()
    {
        var courses = await _uow.Courses.GetAllAsync();
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task<IEnumerable<CourseDto>> GetByInstructorAsync(int instructorId)
    {
        var courses = await _uow.Courses.Query()
            .Include(c => c.Instructor).ThenInclude(i => i.User)
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    // --- CURRICULUM & MEDIA --- //

    public async Task<CourseSectionDto> AddSectionAsync(int courseId, string title)
    {
        var section = new CourseSection
        {
            CourseId = courseId,
            Title = title,
            OrderIndex = await _uow.CourseSections.CountAsync(s => s.CourseId == courseId) + 1
        };

        await _uow.CourseSections.AddAsync(section);
        await _uow.SaveChangesAsync();

        return _mapper.Map<CourseSectionDto>(section);
    }

    public async Task<CourseLessonDto> AddLessonAsync(int sectionId, CreateLessonDto dto)
    {
        var lesson = new CourseLesson
        {
            SectionId = sectionId,
            Title = dto.Title,
            VideoPath = dto.VideoPath,
            VideoKey = dto.VideoKey,
            DurationMinutes = dto.DurationMinutes,
            IsFreePreview = dto.IsFreePreview,
            Status = LessonStatus.Published,
            OrderIndex = await _uow.CourseLessons.CountAsync(l => l.SectionId == sectionId) + 1
        };

        await _uow.CourseLessons.AddAsync(lesson);
        
        // Update Course Total Duration & Lessons
        var section = await _uow.CourseSections.GetByIdAsync(sectionId);
        if (section != null)
        {
            var course = await _uow.Courses.GetByIdAsync(section.CourseId);
            if (course != null)
            {
                course.TotalDuration += (dto.DurationMinutes * 60); // Save as seconds
                course.TotalLessons += 1;
                _uow.Courses.Update(course);
            }
        }

        await _uow.SaveChangesAsync();
        return _mapper.Map<CourseLessonDto>(lesson);
    }

    public async Task<CourseDto> UpdateMediaAsync(int courseId, string? thumbnailUrl, string? thumbnailKey, string? promoUrl, string? promoKey)
    {
        var course = await _uow.Courses.GetByIdAsync(courseId)
            ?? throw new KeyNotFoundException("Course not found");

        if (thumbnailUrl != null) course.ThumbnailUrl = thumbnailUrl;
        if (thumbnailKey != null) course.ThumbnailKey = thumbnailKey;
        if (promoUrl != null) course.PromoVideoUrl = promoUrl;
        if (promoKey != null) course.PromoVideoKey = promoKey;

        course.UpdatedAt = DateTime.UtcNow;
        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }
}