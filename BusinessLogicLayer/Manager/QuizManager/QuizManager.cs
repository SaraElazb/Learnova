using AutoMapper;
using BusinessLogicLayer.DTOs.QuizDtos;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.QuizManager
{
    public class QuizManager : IQuizManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuizManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuizDto>> FindAllAsync()
        {
            var quizzes = await _unitOfWork.GetRepository<Quiz>()
                .FindAll()
                .Include(q => q.Lesson)
                .Include(q => q.Questions)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuizDto>>(quizzes);
        }

        public async Task<QuizDto> FindAsync(int id)
        {
            var quiz = await _unitOfWork.GetRepository<Quiz>()
                .FindByCondition(q => q.Quiz_ID == id)
                .Include(q => q.Lesson)
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync();

            return _mapper.Map<QuizDto>(quiz);
        }

        public async Task<QuizDto> GetByIdAsync(int id)
        {
            var quiz = await _unitOfWork.GetRepository<Quiz>()
                .FindByCondition(q => q.Quiz_ID == id)
                .Include(q => q.Lesson)
                .FirstOrDefaultAsync();

            return _mapper.Map<QuizDto>(quiz);
        }

        public async Task<bool> CreateQuizAsync(QuizRequest model)
        {
            try
            {
                var quiz = _mapper.Map<Quiz>(model);
                
                _unitOfWork.GetRepository<Quiz>().Create(quiz);
                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditQuizAsync(int id, QuizRequest model)
        {
            try
            {
                var quiz = await _unitOfWork.GetRepository<Quiz>()
                    .FindByCondition(q => q.Quiz_ID == id)
                    .FirstOrDefaultAsync();

                if (quiz == null)
                    return false;

                quiz.Title = model.Title;
                quiz.Instructions = model.Instructions;
                quiz.Passing_score = model.Passing_score;
                quiz.Lesson_ID = model.Lesson_ID;

                _unitOfWork.GetRepository<Quiz>().Update(quiz);
                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SoftDelete(QuizDto quiz)
        {
            try
            {
                var quizEntity = await _unitOfWork.GetRepository<Quiz>()
                    .FindByCondition(q => q.Quiz_ID == quiz.Quiz_ID)
                    .FirstOrDefaultAsync();

                if (quizEntity == null)
                    return false;

                _unitOfWork.GetRepository<Quiz>().Delete(quizEntity);
                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByLessonAsync(int lessonId)
        {
            var quizzes = await _unitOfWork.GetRepository<Quiz>()
                .FindByCondition(q => q.Lesson_ID == lessonId)
                .Include(q => q.Lesson)
                .Include(q => q.Questions)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuizDto>>(quizzes);
        }
    }
} 