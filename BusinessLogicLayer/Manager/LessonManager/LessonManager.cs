using AutoMapper;
using BusinessLogicLayer.DTOs.LessonDtos;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Manager.LessonManager
{
    public class LessonManager : ILessonManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LessonManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LessonDto>> FindAllAsync()
        {
            try
            {
                var lessons = await _unitOfWork.GetRepository<Lesson>()
                    .FindAll()
                    .Include(l => l.Course)
                    .Include(l => l.Quiz)
                    .ToListAsync();

                Console.WriteLine($"Found {lessons.Count} lessons in database");
                foreach (var lesson in lessons)
                {
                    Console.WriteLine($"Lesson ID: {lesson.Lesson_ID}, Title: {lesson.Title}, Course ID: {lesson.Course_ID}");
                }

                return _mapper.Map<IEnumerable<LessonDto>>(lessons);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindAllAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return Enumerable.Empty<LessonDto>();
            }
        }

        public async Task<LessonDto> FindAsync(int id)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>()
                .FindByCondition(l => l.Lesson_ID == id)
                .Include(l => l.Course)
                .Include(l => l.Quiz)
                .FirstOrDefaultAsync();

            return _mapper.Map<LessonDto>(lesson);
        }

        public async Task<LessonDto> GetByIdAsync(int id)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>()
                .FindByCondition(l => l.Lesson_ID == id)
                .Include(l => l.Course)
                .FirstOrDefaultAsync();

            return _mapper.Map<LessonDto>(lesson);
        }

        public async Task<bool> CreateLessonAsync(LessonRequest model)
        {
            try
            {
                var lesson = _mapper.Map<Lesson>(model);
                
                Console.WriteLine($"Mapped LessonRequest to Lesson - Title: {lesson.Title}, Course ID: {lesson.Course_ID}");
                
                _unitOfWork.GetRepository<Lesson>().Create(lesson);
                await _unitOfWork.SaveAsync();
                
                Console.WriteLine($"Lesson created successfully with ID: {lesson.Lesson_ID}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateLessonAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                return false;
            }
        }

        public async Task<bool> EditLessonAsync(int id, LessonRequest model)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>()
                    .FindByCondition(l => l.Lesson_ID == id)
                    .FirstOrDefaultAsync();

                if (lesson == null)
                    return false;

                lesson.Title = model.Title;
                lesson.Description = model.Description;
                lesson.Duration = model.Duration;
                lesson.VideoUri = model.VideoUri;
                lesson.LessonOrder = model.LessonOrder;
                lesson.Course_ID = model.Course_ID;
                lesson.Quiz_ID = model.Quiz_ID;

                _unitOfWork.GetRepository<Lesson>().Update(lesson);
                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SoftDelete(LessonDto lesson)
        {
            try
            {
                var lessonEntity = await _unitOfWork.GetRepository<Lesson>()
                    .FindByCondition(l => l.Lesson_ID == lesson.Lesson_ID)
                    .FirstOrDefaultAsync();

                if (lessonEntity == null)
                    return false;

                _unitOfWork.GetRepository<Lesson>().Delete(lessonEntity);
                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<LessonDto>> GetLessonsByCourseAsync(int courseId)
        {
            var lessons = await _unitOfWork.GetRepository<Lesson>()
                .FindByCondition(l => l.Course_ID == courseId)
                .Include(l => l.Course)
                .Include(l => l.Quiz)
                .OrderBy(l => l.LessonOrder)
                .ToListAsync();

            return _mapper.Map<IEnumerable<LessonDto>>(lessons);
        }

        public async Task<bool> MarkLessonAsCompleteAsync(int lessonId, string userId)
        {
            try
            {
                // Check if this lesson exists
                var lesson = await _unitOfWork.GetRepository<Lesson>()
                    .FindByCondition(l => l.Lesson_ID == lessonId)
                    .FirstOrDefaultAsync();

                if (lesson == null)
                    return false;

                // Check if the lesson is already marked as complete
                var existingStudy = await _unitOfWork.GetRepository<Studies>()
                    .FindByCondition(s => s.Lesson_ID == lessonId && s.User_ID == userId)
                    .FirstOrDefaultAsync();

                if (existingStudy != null)
                {
                    // Already completed, just update the completion date if needed
                    if (existingStudy.CompletionDate == null)
                    {
                        existingStudy.CompletionDate = DateTime.Now;
                        existingStudy.Status = "Completed";
                        _unitOfWork.GetRepository<Studies>().Update(existingStudy);
                        await _unitOfWork.SaveAsync();
                    }
                    return true;
                }

                // Create new completion record
                var studiesRecord = new Studies
                {
                    User_ID = userId,
                    Lesson_ID = lessonId,
                    Status = "Completed",
                    Score = 0, // Default score
                    CompletionDate = DateTime.Now
                };

                _unitOfWork.GetRepository<Studies>().Create(studiesRecord);
                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkLessonAsCompleteAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<int>> GetCompletedLessonIdsForCourseAsync(int courseId, string userId)
        {
            try
            {
                // Get all lessons for this course
                var courseLessons = await _unitOfWork.GetRepository<Lesson>()
                    .FindByCondition(l => l.Course_ID == courseId)
                    .Select(l => l.Lesson_ID)
                    .ToListAsync();

                if (!courseLessons.Any())
                    return Enumerable.Empty<int>();

                // Get completed lessons for this user and course
                var completedLessons = await _unitOfWork.GetRepository<Studies>()
                    .FindByCondition(s => s.User_ID == userId && courseLessons.Contains(s.Lesson_ID))
                    .Select(s => s.Lesson_ID)
                    .ToListAsync();

                return completedLessons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCompletedLessonIdsForCourseAsync: {ex.Message}");
                return Enumerable.Empty<int>();
            }
        }
    }
} 