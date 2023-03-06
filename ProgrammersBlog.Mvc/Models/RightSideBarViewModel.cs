using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Models
{
    public class RightSideBarViewModel
    {
        public IList<Article> Articles { get; set; }
        public IList<Category> Categories { get; set; }
    }
}
