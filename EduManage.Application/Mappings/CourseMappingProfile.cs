// Application/Mappings/CourseMappingProfile.cs
using AutoMapper;
using EduManage.Application.DTOs.Courses;
using EduManage.Core.Entities;

namespace EduManage.Application.Mappings;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(d => d.InstructorName,
                       o => o.MapFrom(s => s.Instructor != null ? s.Instructor.User.FullName : string.Empty))
            .ForMember(d => d.CategoryName,
                       o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.Level,
                       o => o.MapFrom(s => s.Level.ToString()))
            .ForMember(d => d.Status,
                       o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateCourseDto, Course>();
        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val is not null));

        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(d => d.CourseTitle,
                       o => o.MapFrom(s => s.Course != null ? s.Course.Title : string.Empty))
            .ForMember(d => d.Status,
                       o => o.MapFrom(s => s.Status.ToString()));
    }
}