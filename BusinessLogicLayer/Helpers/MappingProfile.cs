using AutoMapper;
using BusinessLogicLayer.DTOs.CategoryDtos;
using BusinessLogicLayer.DTOs.CourseDtos;
using BusinessLogicLayer.DTOs.LessonDtos;
using BusinessLogicLayer.DTOs.OrderDtos;
using BusinessLogicLayer.DTOs.QuizDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryRequest, Category>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore());
            CreateMap<Category, CategoryRequest>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            //////////////////

            CreateMap<Course, CourseDTO>()
                .ReverseMap();

            CreateMap<CourseRequest, Course>()
                .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.InstructorId));

            CreateMap<Course, CourseRequest>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.CategorySelectList, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.InstructorId));

            //////////////////

            CreateMap<Lesson, LessonDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title));
            
            CreateMap<LessonDto, Lesson>();
            CreateMap<LessonDto, LessonRequest>().ReverseMap();
            CreateMap<Lesson, LessonRequest>().ReverseMap();

            CreateMap<Quiz, QuizDto>()
                .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Lesson.Title));
            
            CreateMap<QuizDto, Quiz>();
            CreateMap<QuizDto, QuizRequest>().ReverseMap();
            CreateMap<Quiz, QuizRequest>().ReverseMap();

            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText));
            
            CreateMap<QuestionDto, Question>();
            CreateMap<QuestionDto, QuestionRequest>().ReverseMap();
            CreateMap<Question, QuestionRequest>().ReverseMap();

            CreateMap<Answer, AnswerDto>()
                .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => 
                    src.Question != null && src.Answers == src.Question.RightAns));
            
            CreateMap<AnswerDto, Answer>();

            //////////////////

            // Order mappings
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}
