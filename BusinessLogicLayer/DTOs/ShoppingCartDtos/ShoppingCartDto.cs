using BusinessLogicLayer.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.ShoppingCartDtos
{
    public class ShoppingCartDto
    {
        public List<CourseDTO> Items { get; set; } = new List<CourseDTO>();
    }
}
