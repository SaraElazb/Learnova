using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Course> Courses { get; }
        IGenericRepository<Quiz> Quizzes { get; }
        IGenericRepository<Question> Questions { get; }
        IGenericRepository<Lesson> Lessons { get; }
        IGenericRepository<Enrollment> Enrollments { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Answer> Answers { get; }
        IGenericRepository<Certificate> Certificates { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<Receive> Receives { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<Studies> Studies { get; }
        IGenericRepository<Submission> Submissions { get; }

        IGenericRepository<Course> CourseRepository { get; }
        Task<int> CompleteAsync();

       
    }


}
