using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Course> Courses { get; }
        public IGenericRepository<Quiz> Quizzes { get; }
        public IGenericRepository<Question> Questions { get; }
        public IGenericRepository<Lesson> Lessons { get; }
        public IGenericRepository<Enrollment> Enrollments { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Answer> Answers { get; }
        public IGenericRepository<Certificate> Certificates { get; }
        public IGenericRepository<Notification> Notifications { get; }
        public IGenericRepository<Payment> Payments { get; }
        public IGenericRepository<Receive> Receives { get; }
        public IGenericRepository<Review> Reviews { get; }
        public IGenericRepository<Role> Roles { get; }
        public IGenericRepository<Studies> Studies { get; }
        public IGenericRepository<Submission> Submissions { get; }

        public object CourseRepository => throw new NotImplementedException();

        IGenericRepository<Course> IUnitOfWork.CourseRepository => throw new NotImplementedException();

        public UnitOfWork(DbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(_context);
            Courses = new GenericRepository<Course>(_context);
            Quizzes = new GenericRepository<Quiz>(_context);
            Questions = new GenericRepository<Question>(_context);
            Lessons = new GenericRepository<Lesson>(_context);
            Enrollments = new GenericRepository<Enrollment>(_context);
            Categories = new GenericRepository<Category>(_context);
            Answers = new GenericRepository<Answer>(_context);
            Certificates = new GenericRepository<Certificate>(_context);
            Notifications = new GenericRepository<Notification>(_context);
            Payments = new GenericRepository<Payment>(_context);
            Receives = new GenericRepository<Receive>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Roles = new GenericRepository<Role>(_context);
            Studies = new GenericRepository<Studies>(_context);
            Submissions = new GenericRepository<Submission>(_context);
        }


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
       
    }

}
