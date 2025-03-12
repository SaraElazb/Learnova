using Microsoft.EntityFrameworkCore;
using System;

namespace Grad_Project.Models
{
    public class ELearningDbContext : DbContext
    {
        public ELearningDbContext(DbContextOptions<ELearningDbContext> options) : base(options)
        {
        }
        
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Receive> Receives { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Studies> Studies { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== Composite Keys ==========
            
            
            modelBuilder.Entity<Studies>()
                .HasKey(s => new { s.User_ID, s.Lesson_ID });
            
            modelBuilder.Entity<Receive>()
                .HasKey(r => new { r.User_ID, r.Notification_ID, r.Datetime });

            // ========== One-to-One Relationships ==========

            modelBuilder.Entity<Enrollment>()
               .HasOne(e => e.Certificate)
               .WithOne(c => c.Enrollment)
               .HasForeignKey<Certificate>(c => c.EnrollmentID)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Payment)
                .WithOne(p => p.Enrollment)
                .HasForeignKey<Payment>(p => p.Enrollment_ID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Quiz)
                .WithOne(q => q.Lesson)
                .HasForeignKey<Quiz>(q => q.Lesson_ID)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== One-to-Many Relationships ==========

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.Role_ID)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Courses)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.Category_ID)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.Course_ID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Course)
                .HasForeignKey(r => r.Course_ID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.Course_ID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Enrollments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.User_ID)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Submissions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.User_ID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Submissions)
                .WithOne(s => s.Quiz)
                .HasForeignKey(s => s.Quiz_ID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionID)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<User>()
                .HasMany<Quiz>()
                .WithOne(q => q.User)
                .HasForeignKey(q => q.User_ID)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // ========== Many-to-Many Relationships ==========
            
            modelBuilder.Entity<Studies>()
                .HasOne(s => s.User)
                .WithMany(u => u.Studies)
                .HasForeignKey(s => s.User_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Studies>()
                .HasOne(s => s.Lesson)
                .WithMany(l => l.Studies)
                .HasForeignKey(s => s.Lesson_ID)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Receive>()
                .HasOne(r => r.User)
                .WithMany(u => u.Receives)
                .HasForeignKey(r => r.User_ID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Receive>()
                .HasOne(r => r.Notification)
                .WithMany(n => n.Receives)
                .HasForeignKey(r => r.Notification_ID)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== Additional Configuration ==========
            
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            modelBuilder.Entity<Course>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.Enrollment_date)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.Registration_date)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Transaction_date)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Submission>()
                .Property(s => s.SubmissionDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}