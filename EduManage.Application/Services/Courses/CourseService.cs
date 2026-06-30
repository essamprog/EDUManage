// Application/Services/Courses/CourseService.cs
using AutoMapper;
using EduManage.Application.DTOs.Common;
using EduManage.Application.DTOs.Courses;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;

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
        var all = await _uow.Courses.FindAsync(c =>
            c.Status == CourseStatus.Published &&
            (filter.Search == null || c.Title.Contains(filter.Search)) &&
            (filter.CategoryId == null || c.CategoryId == filter.CategoryId) &&
            (filter.Level == null || c.Level == filter.Level) &&
            (filter.MinPrice == null || c.Price >= filter.MinPrice) &&
            (filter.MaxPrice == null || c.Price <= filter.MaxPrice));

        var sorted = filter.SortBy switch
        {
            "popular" => all.OrderByDescending(c => c.TotalStudents),
            "rating" => all.OrderByDescending(c => c.AverageRating),
            "price" => all.OrderBy(c => c.Price),
            _ => all.OrderByDescending(c => c.CreatedAt),
        };

        var total = sorted.Count();
        var items = sorted
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

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

    public Task<IEnumerable<CourseDto>> GetByInstructorAsync(int instructorId)
    {
        throw new NotImplementedException();
    }
}