using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Mvc.Models;

namespace ProgrammersBlog.Mvc.ViewComponents
{
    public class RightSideBarViewComponent:ViewComponent
    {
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleService;

        public RightSideBarViewComponent(ICategoryService categoryService, IArticleService articleService)
        {
            _categoryService = categoryService;
            _articleService = articleService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories =await _categoryService.GetAllByNonDeleted();
            var articles = await _articleService.GetAllViewCountAsync(isAscending: false, takeSize: 5);
            return View(new RightSideBarViewModel
            {
                Articles=articles.Data.Articles,
                Categories=categories.Data.Categories
            });
        }
    }
}
