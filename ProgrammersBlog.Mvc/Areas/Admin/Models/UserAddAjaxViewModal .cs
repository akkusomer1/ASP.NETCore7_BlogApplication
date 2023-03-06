using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Dtos.UserDto;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class UserAddAjaxViewModal
    {
        public UserAddDto UserAddDto { get; set; }
        public string  UserAddPartial { get; set; }
        public UserDto UserDto { get; set; }
    }
}
