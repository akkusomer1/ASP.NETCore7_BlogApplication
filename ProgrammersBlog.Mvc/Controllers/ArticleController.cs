using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Mvc.Filter;
using ProgrammersBlog.Mvc.Models;

namespace ProgrammersBlog.Mvc.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ArticleRightSideBarWidgetOptions _options;
        public ArticleController(IArticleService articleService,IOptionsSnapshot<ArticleRightSideBarWidgetOptions>  options)
        {
            _articleService = articleService;
            _options = options.Value;
        }


        [ViewCountFilter]
        [HttpGet]
        public async Task<IActionResult> Detail(int articleId)
        {
            var result = await _articleService.Get(articleId);
            if (result.ResultStatus == ResultStatusType.Success)
            {
             var userArticles=   await _articleService.GetAllByUserIdOnFilter(result.Data.Article.UserId, _options.FilterBy, _options.OrderBy, _options.IsAscending, _options.TakeSize, _options.CategoryId, _options.StartAt, _options.EndAt, _options.MinViewCount, _options.MaxViewCount, _options.MinCommentCount, _options.MaxCommentCount);

                //await _articleService.IncreaseViewCountAsync(articleId);
                return View(new ArticleDetailViewModel
                {
                    ArticleDto=result.Data,
                    ArticleDetailRightSideBarViewModel=new ArticleDetailRightSideBarViewModel
                    {
                        ArticleListDto=userArticles.Data,
                        User= result.Data.Article.User,
                        Header= _options.Header
                    }
                });
            }
            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Search(string keyword, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            var result = await _articleService.SearchAsync(keyword, currentPage, pageSize, isAscending);

            if (result.ResultStatus == ResultStatusType.Success)
            {
                return View(new ArticleSearchViewModel
                {
                    ArticleListDto = result.Data,
                    Keyword = keyword
                });
            }
            return NotFound();
        }
    }
}
