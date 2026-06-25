using EduManage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Infrastructure.Context.Configurations
{
    // QaQuestionConfiguration.cs
    public class QaQuestionConfiguration : IEntityTypeConfiguration<QaQuestion>
    {
        public void Configure(EntityTypeBuilder<QaQuestion> builder)
        {
            builder.ToTable("QaQuestions");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.QuestionText).IsRequired().HasColumnType("text");

            builder.HasOne(q => q.Lesson)
                   .WithMany(l => l.Questions)
                   .HasForeignKey(q => q.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.User)
                   .WithMany()
                   .HasForeignKey(q => q.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing للـ Replies
            builder.HasOne(q => q.Parent)
                   .WithMany(q => q.Replies)
                   .HasForeignKey(q => q.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
