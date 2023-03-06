using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class UserWithRolesViewModal
    {
        public User  User { get; set; }
        public IList<string>  Roles { get; set; }
    }
}
