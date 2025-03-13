using AutoMapper;
using BusinessLogicLayer.DTOs.CategoryDtos;
using BusinessLogicLayer.DTOs.CourseDtos;
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
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Category_Name));

            CreateMap<CourseRequest, Course>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore());

            CreateMap<Course, CourseRequest>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.CategorySelectList, opt => opt.Ignore());
        }
    }
}
