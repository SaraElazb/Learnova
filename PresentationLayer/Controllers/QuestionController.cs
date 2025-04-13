using AutoMapper;
using BusinessLogicLayer.DTOs.QuizDtos;
using BusinessLogicLayer.Manager.QuestionManager;
using BusinessLogicLayer.Manager.QuizManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PresentationLayer.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IQuestionManager _questionManager;
        private readonly IQuizManager _quizManager;

        public QuestionController(
            IMapper mapper,
            IQuestionManager questionManager,
            IQuizManager quizManager)
        {
            _mapper = mapper;
            _questionManager = questionManager;
            _quizManager = quizManager;
        }

        private async Task<IEnumerable<SelectListItem>> GetQuizzesAsync()
        {
            var quizzes = await _quizManager.FindAllAsync();
            return quizzes.Select(q => new SelectListItem
            {
                Value = q.Quiz_ID.ToString(),
                Text = q.Title
            });
        }

        public async Task<IActionResult> Index(int quizId)
        {
            var questionDtos = await _questionManager.GetQuestionsByQuizAsync(quizId);
            ViewBag.QuizId = quizId;
            var quiz = await _quizManager.FindAsync(quizId);
            if (quiz != null)
            {
                ViewBag.QuizTitle = quiz.Title;
            }
            return View(questionDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var questionDto = await _questionManager.FindAsync(id);
            if (questionDto == null) return NotFound();

            return View(questionDto);
        }

        public async Task<IActionResult> Create(int quizId)
        {
            var model = new QuestionRequest
            {
                QuizID = quizId,
                Answers = new List<string> { "", "", "", "" }
            };
            
            ViewBag.Quizzes = await GetQuizzesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionRequest model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Quizzes = await GetQuizzesAsync();
                return View(model);
            }

            await _questionManager.CreateQuestionAsync(model);
            return RedirectToAction(nameof(Index), new { quizId = model.QuizID });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionManager.GetByIdAsync(id);
            if (question == null) return NotFound();

            var model = _mapper.Map<QuestionRequest>(question);
            
            var fullQuestion = await _questionManager.FindAsync(id);
            model.Answers = fullQuestion.Answers.Select(a => a.Answers).ToList();
            
            ViewBag.Quizzes = await GetQuizzesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, QuestionRequest model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Quizzes = await GetQuizzesAsync();
                return View(model);
            }

            var success = await _questionManager.EditQuestionAsync(id, model);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index), new { quizId = model.QuizID });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionManager.GetByIdAsync(id);
            if (question == null) return NotFound();

            var quizId = question.QuizID;
            await _questionManager.DeleteQuestion(id);

            return RedirectToAction(nameof(Index), new { quizId });
        }
    }
} 