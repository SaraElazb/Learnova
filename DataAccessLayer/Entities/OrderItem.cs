using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
    }
}
