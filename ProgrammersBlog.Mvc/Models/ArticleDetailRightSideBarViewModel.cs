using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Models
{
    public class ArticleDetailRightSideBarViewModel
    {
        public string Header { get; set; }
        public ArticleListDto ArticleListDto { get; set; }
        public User User { get; set; }
    }
}
