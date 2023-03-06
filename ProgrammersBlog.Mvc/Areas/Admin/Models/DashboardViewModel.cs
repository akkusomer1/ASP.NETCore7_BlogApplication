using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    //Article.GetAll()  metodu bize ArticleListDto döndürür.
    public class DashboardViewModel
    {
        public int CategoryCount { get; set; }
        public int ArticleCount { get; set; }
        public int CommentCount { get; set; }
        public int UserCount { get; set; }

        public ArticleListDto Articles { get; set; }
    }
}
