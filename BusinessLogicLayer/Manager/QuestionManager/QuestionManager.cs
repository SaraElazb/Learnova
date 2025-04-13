using AutoMapper;
using BusinessLogicLayer.DTOs.QuizDtos;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.QuestionManager
{
    public class QuestionManager : IQuestionManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuestionDto>> FindAllAsync()
        {
            var questions = await _unitOfWork.GetRepository<Question>()
                .FindAll()
                .Include(q => q.Quiz)
                .Include(q => q.Answers)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }

        public async Task<QuestionDto> FindAsync(int id)
        {
            var question = await _unitOfWork.GetRepository<Question>()
                .FindByCondition(q => q.QuestionID == id)
                .Include(q => q.Quiz)
                .Include(q => q.Answers)
                .FirstOrDefaultAsync();

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<QuestionDto> GetByIdAsync(int id)
        {
            var question = await _unitOfWork.GetRepository<Question>()
                .FindByCondition(q => q.QuestionID == id)
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync();

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<bool> CreateQuestionAsync(QuestionRequest model)
        {
            try
            {
                var question = new Question
                {
                    QuizID = model.QuizID,
                    Score = model.Score,
                    RightAns = model.RightAns
                };

                _unitOfWork.GetRepository<Question>().Create(question);
                await _unitOfWork.SaveAsync();

                // Create answers for the question
                if (model.Answers != null && model.Answers.Any())
                {
                    foreach (var answerText in model.Answers)
                    {
                        var answer = new Answer
                        {
                            QuestionID = question.QuestionID,
                            Answers = answerText
                        };

                        _unitOfWork.GetRepository<Answer>().Create(answer);
                    }

                    await _unitOfWork.SaveAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditQuestionAsync(int id, QuestionRequest model)
        {
            try
            {
                var question = await _unitOfWork.GetRepository<Question>()
                    .FindByCondition(q => q.QuestionID == id)
                    .FirstOrDefaultAsync();

                if (question == null)
                    return false;

                question.Score = model.Score;
                question.RightAns = model.RightAns;
                question.QuizID = model.QuizID;

                _unitOfWork.GetRepository<Question>().Update(question);

                // Update answers
                var existingAnswers = await _unitOfWork.GetRepository<Answer>()
                    .FindByCondition(a => a.QuestionID == id)
                    .ToListAsync();

                // Delete existing answers
                foreach (var answer in existingAnswers)
                {
                    _unitOfWork.GetRepository<Answer>().Delete(answer);
                }

                // Add new answers
                if (model.Answers != null && model.Answers.Any())
                {
                    foreach (var answerText in model.Answers)
                    {
                        var answer = new Answer
                        {
                            QuestionID = question.QuestionID,
                            Answers = answerText
                        };

                        _unitOfWork.GetRepository<Answer>().Create(answer);
                    }
                }

                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteQuestion(int id)
        {
            try
            {
                var question = await _unitOfWork.GetRepository<Question>()
                    .FindByCondition(q => q.QuestionID == id)
                    .FirstOrDefaultAsync();

                if (question == null)
                    return false;

                // Delete associated answers first
                var answers = await _unitOfWork.GetRepository<Answer>()
                    .FindByCondition(a => a.QuestionID == id)
                    .ToListAsync();

                foreach (var answer in answers)
                {
                    _unitOfWork.GetRepository<Answer>().Delete(answer);
                }

                _unitOfWork.GetRepository<Question>().Delete(question);
                await _unitOfWork.SaveAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByQuizAsync(int quizId)
        {
            var questions = await _unitOfWork.GetRepository<Question>()
                .FindByCondition(q => q.QuizID == quizId)
                .Include(q => q.Answers)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }
    }
} 