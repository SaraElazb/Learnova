using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.UserRolesDtos
{
    public class EditUserRolesDto
    {
        public string UserId { get; set; }
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}
