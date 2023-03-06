using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Mvc.Areas.Admin.Models;

namespace ProgrammersBlog.Mvc.Areas.Admin
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;
        private readonly IArticleService _articleService;
        private readonly UserManager<User> _userManager;

        public HomeController(ICategoryService categoryService, ICommentService commentService, IArticleService articleService, UserManager<User> userManager)
        {
            _categoryService = categoryService;
            _commentService = commentService;
            _articleService = articleService;
            _userManager = userManager;
        }


        [Authorize(Roles="SuperAdmin,AdminArea.Home.Read")]
        public async Task<IActionResult> Index()
        {
            var categoriesCountResult = await _categoryService.CountByNonDeletedAsync();
            var articlesCountResult = await _articleService.CountByNonDeletedAsync();
            var commentCountResult = await _commentService.CountByNonDeletedAsync();
            var userCount = await _userManager.Users.CountAsync();
            var articles = await _articleService.GetAll();


            if (categoriesCountResult.ResultStatus == ResultStatusType.Success && articlesCountResult.ResultStatus == ResultStatusType.Success && commentCountResult.ResultStatus == ResultStatusType.Success && userCount > -1 && articles.ResultStatus == ResultStatusType.Success)
            {
                var dashboardModel = new DashboardViewModel
                {
                    CategoryCount = categoriesCountResult.Data,
                    CommentCount = commentCountResult.Data,
                    ArticleCount = articlesCountResult.Data,
                    UserCount = userCount,
                    Articles = articles.Data
                };
                return View(dashboardModel);
            }
            return NotFound();
        }
    }
}
