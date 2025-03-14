using AutoMapper;
using BusinessLogicLayer.DTOs.CategoryDtos;
using BusinessLogicLayer.DTOs.CourseDtos;
using BusinessLogicLayer.Helpers;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.CourseManager
{
    public class CourseManager : ICourseManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseManager(IUnitOfWork unitOfWork , IMapper mapper , IWebHostEnvironment webHostEnvironment)
        {
             _unitOfWork = unitOfWork;
             _mapper = mapper;
             _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<CourseDTO>> FindAllAsync()
        {
            var courses = await _unitOfWork.Courses.FindAllAsync(c => c.IsActive, include: q => q.Include(c => c.Category));
            var activeCourses = courses.Where(c => c.IsActive);
            var courseDTOs = _mapper.Map<IEnumerable<CourseDTO>>(activeCourses);
            return courseDTOs;
        }

        public async Task<CourseDTO> FindAsync(int id)
        {
            var course = await _unitOfWork.Courses.FindAsync(c => c.Course_ID == id, include: q => q.Include(c => c.Category));
            var courseDTO = _mapper.Map<CourseDTO>(course);
            return courseDTO;
        }
        public async Task<IEnumerable<CourseDTO>> GetCoursesAsync(int? categoryId)
        {
            var coursesQuery = _unitOfWork.Courses.FindAllAsync(c => c.IsActive, q => q.Include(c => c.Category));
            var courses = await coursesQuery;

            if (categoryId.HasValue)
            {
                courses = courses.Where(c => c.Category_ID == categoryId.Value);
            }

            return _mapper.Map<IEnumerable<CourseDTO>>(courses);
        }

        public async Task CreateCourseAsync(CourseRequest model)
        {
            var course = _mapper.Map<Course>(model);

           
            if (model.Image != null)
            {
                course.ImagePath = ImageHelper.SaveImage(model.Image, "CourseImages", _webHostEnvironment);
            }

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<CourseRequest?> GetCourseForEditAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) return null;

            var model = _mapper.Map<CourseRequest>(course);
            model.CategorySelectList = await GetCategoriesAsync();
            return model;
        }

        public async Task<bool> EditCourseAsync(int id, CourseRequest model)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) return false;

            _mapper.Map(model, course);

            if (model.Image != null)
            {
                course.ImagePath = ImageHelper.SaveImage(model.Image, "CourseImages", _webHostEnvironment);
            }

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<IEnumerable<SelectListItem>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.FindAllAsync(c => c.IsActive);
            return categories.Select(c => new SelectListItem
            {
                Value = c.Category_ID.ToString(),
                Text = c.Category_Name
            });
        }

        public Task<Course> GetByIdAsync(int id)
        {
            var course = _unitOfWork.Courses.GetByIdAsync(id);
            return course;
        }

        public async Task SoftDelete(Course course)
        {
            _unitOfWork.Courses.SoftDelete(course);
            await _unitOfWork.CompleteAsync();

        }
    }
}
