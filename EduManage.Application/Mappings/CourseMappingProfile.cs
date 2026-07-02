using AutoMapper;
using EduManage.Application.DTOs.Courses;
using EduManage.Application.DTOs.Financial;
using EduManage.Core.Entities;

namespace EduManage.Application.Mappings;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        // Course → CourseDto
        CreateMap<Course, CourseDto>()
            .ForMember(d => d.InstructorName,
                       o => o.MapFrom(s => s.Instructor != null
                           ? s.Instructor.User.FullName
                           : string.Empty))
            .ForMember(d => d.CategoryName,
                       o => o.MapFrom(s => s.Category != null
                           ? s.Category.Name
                           : null))
            .ForMember(d => d.Level,
                       o => o.MapFrom(s => s.Level.ToString()))
            .ForMember(d => d.Status,
                       o => o.MapFrom(s => s.Status.ToString()));

        // CreateCourseDto → Course
        CreateMap<CreateCourseDto, Course>();

        // UpdateCourseDto → Course
        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // Enrollment → EnrollmentDto
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(d => d.CourseTitle,
                       o => o.MapFrom(s => s.Course != null
                           ? s.Course.Title
                           : string.Empty))
            .ForMember(d => d.ThumbnailUrl,
                       o => o.MapFrom(s => s.Course != null
                           ? s.Course.ThumbnailUrl
                           : null))
            .ForMember(d => d.Status,
                       o => o.MapFrom(s => s.Status.ToString()));
    }
}