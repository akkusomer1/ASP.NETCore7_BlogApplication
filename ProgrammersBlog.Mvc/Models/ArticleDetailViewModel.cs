using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;

namespace ProgrammersBlog.Mvc.Models
{

    public class ArticleDetailViewModel
    {
        public ArticleDto ArticleDto { get; set; }
        public  ArticleDetailRightSideBarViewModel ArticleDetailRightSideBarViewModel { get; set; }

    }
}
