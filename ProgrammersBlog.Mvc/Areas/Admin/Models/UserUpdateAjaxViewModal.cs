using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Dtos.UserDto;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class UserUpdateAjaxViewModal
    {
        public UserUpdateDto UserUpdateDto { get; set; }
        public string  UserUpdatePartial { get; set; }
        public UserDto UserDto { get; set; }
    }
}
