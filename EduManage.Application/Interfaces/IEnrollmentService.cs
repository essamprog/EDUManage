using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EduManage.Application.DTOs;
using EduManage.Application.DTOs.Courses;

namespace EduManage.Application.Interfaces
{
    public interface IEnrollmentService
{
    Task<EnrollmentDto> EnrollAsync(int studentId, int courseId);
    Task<IEnumerable<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId);
    Task<bool> MarkLessonCompleteAsync(int studentId, int lessonId, int courseId);
    Task<byte> GetCourseProgressAsync(int studentId, int courseId);
}
}
