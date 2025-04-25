using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.OrderDtos
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } 
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
