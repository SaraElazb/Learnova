using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
        public class Student : User
        {
            public string? NationalId { get; set; }
            public string? Grade { get; set; }
        }
    
}
