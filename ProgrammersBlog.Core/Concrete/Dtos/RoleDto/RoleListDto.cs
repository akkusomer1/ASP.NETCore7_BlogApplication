using ProgrammersBlog.Core.Concrete.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Concrete.Dtos.RoleDto
{
    public class RoleListDto
    {
        public IList<Role> Roles { get; set; }
    }
}
