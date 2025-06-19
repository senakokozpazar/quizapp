using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace quizapp.data.Models;

public partial class QuizappContext : DbContext
{
    public QuizappContext()
    {
    }

    public QuizappContext(DbContextOptions<QuizappContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersAnswer> UsersAnswers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=quizapp;\nUser Id=SA;Password=reallyStrongPwd123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Text)
                .HasMaxLength(50)
                .HasColumnName("text");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_Answers_Questions");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.QuizId).HasColumnName("quiz_id");
            entity.Property(e => e.Text)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("text");

            entity.HasOne(d => d.Quiz).WithMany(p => p.InverseQuiz)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK_Questions_Quizzes");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.Property(e => e.QuizId).HasColumnName("quiz_id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.QuizTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("quiz_title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("password");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<UsersAnswer>(entity =>
        {
            entity.Property(e => e.UsersAnswerId).HasColumnName("users_answer_id");
            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Answer).WithMany(p => p.UsersAnswers)
                .HasForeignKey(d => d.AnswerId)
                .HasConstraintName("FK_UsersAnswers_Answers");

            entity.HasOne(d => d.User).WithMany(p => p.UsersAnswers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UsersAnswers_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
