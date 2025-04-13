using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.AccountDto
{
    public class TeacherRegisterDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Password { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }

    }
}
