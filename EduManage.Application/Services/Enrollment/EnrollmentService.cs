using AutoMapper;
using EduManage.Application.DTOs.Courses;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

// Application/Services/Enrollment/EnrollmentService.cs
namespace EduManage.Application.Services.Enrollment;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public EnrollmentService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<EnrollmentDto> EnrollAsync(int studentId, int courseId)
    {
        var exists = await _uow.Enrollments.AnyAsync(
            e => e.StudentId == studentId && e.CourseId == courseId);

        if (exists)
            throw new InvalidOperationException("You are already enrolled in this course.");

        var enrollment = new EduManage.Core.Entities.Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            Progress = 0,
            Status = EnrollmentStatus.InProgress,
            EnrolledAt = DateTime.UtcNow,
        };

        await _uow.Enrollments.AddAsync(enrollment);

        // تحديث عدد الطلاب في الكورس
        var course = await _uow.Courses.GetByIdAsync(courseId);
        if (course is not null)
        {
            course.TotalStudents++;
            _uow.Courses.Update(course);
        }

        await _uow.SaveChangesAsync();
        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<IEnumerable<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId)
    {
        var enrollments = await _uow.Enrollments
            .FindAsync(e => e.StudentId == studentId);
        return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
    }

    public async Task<bool> MarkLessonCompleteAsync(int studentId, int lessonId, int courseId)
    {
        var progresses = await _uow.LessonProgresses
            .FindAsync(lp => lp.StudentId == studentId && lp.LessonId == lessonId);

        var progress = progresses.FirstOrDefault();

        if (progress is null)
        {
            await _uow.LessonProgresses.AddAsync(new LessonProgress
            {
                StudentId = studentId,
                LessonId = lessonId,
                CourseId = courseId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
        }
        else
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            progress.UpdatedAt = DateTime.UtcNow;
            _uow.LessonProgresses.Update(progress);
        }

        await _uow.SaveChangesAsync();

        // إعادة حساب الـ Progress %
        await UpdateCourseProgressAsync(studentId, courseId);
        return true;
    }

    public async Task<byte> GetCourseProgressAsync(int studentId, int courseId)
    {
        var enrollments = await _uow.Enrollments
            .FindAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        return enrollments.FirstOrDefault()?.Progress ?? 0;
    }

    private async Task UpdateCourseProgressAsync(int studentId, int courseId)
    {
        var course = await _uow.Courses.GetByIdAsync(courseId);
        if (course is null || course.TotalLessons == 0) return;

        var completed = await _uow.LessonProgresses.CountAsync(
            lp => lp.StudentId == studentId &&
                  lp.CourseId == courseId &&
                  lp.IsCompleted);

        var percent = (byte)Math.Round((double)completed / course.TotalLessons * 100);

        var enrollments = await _uow.Enrollments
            .FindAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        var enrollment = enrollments.FirstOrDefault();
        if (enrollment is null) return;

        enrollment.Progress = percent;

        // لو خلص 100% — اعمله Completed
        if (percent == 100)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;
        }

        _uow.Enrollments.Update(enrollment);
        await _uow.SaveChangesAsync();
    }
}