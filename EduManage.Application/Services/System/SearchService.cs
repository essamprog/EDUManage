// Application/Services/System/SearchService.cs
using AutoMapper;
using EduManage.Application.DTOs.Common;
using EduManage.Application.DTOs.Courses;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;

namespace EduManage.Application.Services.System;

public class SearchService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SearchService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<CourseDto>> SearchCoursesAsync(CourseFilterDto filter)
    {
        var all = await _uow.Courses.FindAsync(c =>
            c.Status == CourseStatus.Published &&
            (string.IsNullOrEmpty(filter.Search) ||
             c.Title.Contains(filter.Search) ||
             (c.Description != null && c.Description.Contains(filter.Search))) &&
            (filter.CategoryId == null || c.CategoryId == filter.CategoryId) &&
            (filter.Level == null || c.Level == filter.Level) &&
            (filter.MinPrice == null || c.Price >= filter.MinPrice) &&
            (filter.MaxPrice == null || c.Price <= filter.MaxPrice));

        var sorted = filter.SortBy switch
        {
            "popular" => all.OrderByDescending(c => c.TotalStudents),
            "rating" => all.OrderByDescending(c => c.AverageRating),
            "price-asc" => all.OrderBy(c => c.Price),
            "price-desc" => all.OrderByDescending(c => c.Price),
            _ => all.OrderByDescending(c => c.CreatedAt),
        };

        var total = sorted.Count();
        var items = sorted
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return new PagedResult<CourseDto>
        {
            Items = _mapper.Map<IEnumerable<CourseDto>>(items),
            Total = total,
            Page = filter.Page,
            PageSize = filter.PageSize,
        };
    }
}