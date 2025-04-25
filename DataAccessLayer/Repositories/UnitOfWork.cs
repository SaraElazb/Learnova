using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ELearningDbContext _context;

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Course> Courses { get; private set; }
        public IGenericRepository<Quiz> Quizzes { get; private set; }
        public IGenericRepository<Question> Questions { get; private set; }
        public IGenericRepository<Lesson> Lessons { get; private set; }
        public IGenericRepository<Enrollment> Enrollments { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Answer> Answers { get; private set; }
        public IGenericRepository<Certificate> Certificates { get; private set; }
        public IGenericRepository<Notification> Notifications { get; private set; }
        public IGenericRepository<Payment> Payments { get; private set; }
        public IGenericRepository<Receive> Receives { get; private set; }
        public IGenericRepository<Review> Reviews { get; private set; }
        public IGenericRepository<Role> Roles { get; private set; }
        public IGenericRepository<Studies> Studies { get; private set; }
        public IGenericRepository<Submission> Submissions { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }

        IGenericRepository<Stripe.Climate.Order> IUnitOfWork.Orders => throw new NotImplementedException();


        //public object CourseRepository => throw new NotImplementedException();

        //IGenericRepository<Course> IUnitOfWork.CourseRepository => throw new NotImplementedException();

        public UnitOfWork(ELearningDbContext context)
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
            Orders = new GenericRepository<Order>(_context);
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
            _context.SaveChanges();
        }
        
        // Implementation for the missing methods
        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
