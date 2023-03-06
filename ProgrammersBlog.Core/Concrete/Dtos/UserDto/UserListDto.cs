using ProgrammersBlog.Core.Concrete.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Concrete.Dtos.UserDto
{

    public class UserListDto:BaseDto
    {
      public IList<User> Users { get; set; }
    }
}
