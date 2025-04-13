using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.AccountDto
{
    public class StudentRegisterDto
    {
        
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Password { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public string? NationalId { get; set; }
        public string? Grade { get; set; }
     

    }
}
