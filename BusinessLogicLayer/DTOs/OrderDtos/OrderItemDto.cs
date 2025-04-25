using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.OrderDtos
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public decimal Price { get; set; }
    }
}
