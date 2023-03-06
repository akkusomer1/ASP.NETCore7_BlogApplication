using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Concrete;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IImageHelper _imageHelper;
        private readonly IToastNotification _toastNotification;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, IMapper mapper, IImageHelper imageHelper, UserManager<User> userManager, IToastNotification toastNotification)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _imageHelper = imageHelper;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        [Authorize(Roles = "SuperAdmin,Article.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var result = await _articleService.GetAllByNonDeleted();
            if (result.ResultStatus == ResultStatusType.Success)
            {
                return View(result.Data);
            }
            return NotFound();
        }


        [Authorize(Roles = "SuperAdmin,User.Create")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var result = await _categoryService.GetAllByNonDeleted();

            if (result.ResultStatus == ResultStatusType.Success)
            {
                return View(new ArticleAddViewModel
                {
                    Categories = new SelectList(result.Data.Categories, "Id", "Name")
                });
            }
            return NotFound();
        }

        [Authorize(Roles = "SuperAdmin,User.Create")]
        [HttpPost]
        public async Task<IActionResult> Add(ArticleAddViewModel articleAddVm)
        {
            if (ModelState.IsValid)
            {
                var articleCreateDto = _mapper.Map<ArticleCreateDto>(articleAddVm);
                var imageResult = await _imageHelper.Upload(articleAddVm.Title, articleAddVm.ThumbnailFile, PictureType.Post);
                articleCreateDto.Thumbnail = imageResult.Data.FullName;

                User user = await _userManager.FindByNameAsync(User.Identity.Name!);
                var result = await _articleService.Add(articleCreateDto, user.UserName, user.Id);

                if (result.ResultStatus == ResultStatusType.Success)
                {
                    _toastNotification.AddSuccessToastMessage(result.Message, new ToastrOptions
                    {
                        CloseButton = true,
                        Title = "Başarılı işlem",
                    });

                    return RedirectToAction("Index", "Article");
                }
                ModelState.AddModelError("", result.Message);
            }      
            var catgoryResult = await _categoryService.GetAllByNonDeleted();
            articleAddVm.Categories = new SelectList(catgoryResult.Data.Categories, "Id", "Name");
            return View(articleAddVm);
        }


        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var result = await _articleService.ArticleUpdateDtoAsync(id);
            var categoryResult = await _categoryService.GetAllByNonDeleted();
            if (result.ResultStatus == ResultStatusType.Success && categoryResult.ResultStatus == ResultStatusType.Success)
            {
                ArticleUpdateViewModel vm = _mapper.Map<ArticleUpdateViewModel>(result.Data);
                vm.Categories = new SelectList(categoryResult.Data.Categories, "Id", "Name");
                return View(vm);
            }
            return NotFound();
        }


        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpPost]
        public async Task<IActionResult> Update(ArticleUpdateViewModel articleUpdateVm)
        {

            if (ModelState.IsValid)
            {
                bool isNewThumbnailUploaded = false;

                var oldThumbnail = articleUpdateVm.Thumbnail;


                var updateDto = _mapper.Map<ArticleUpdateDto>(articleUpdateVm);
                var result = await _articleService.Update(updateDto, User.Identity.Name!);

                if (result.ResultStatus == ResultStatusType.Success)
                {
                    if (isNewThumbnailUploaded)
                    {
                        _imageHelper.DeleteImage(oldThumbnail);
                    }
                    _toastNotification.AddSuccessToastMessage(result.Message,new ToastrOptions
                    {
                        CloseButton = true,
                        Title="Başarılı işlem",                     
                    });
                    return RedirectToAction("Index", "Article");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }

            var categoryResult = await _categoryService.GetAllByNonDeleted();
            articleUpdateVm.Categories = new SelectList(categoryResult.Data.Categories, "Id", "Name");
            return View(articleUpdateVm);
        }


        [Authorize(Roles = "SuperAdmin,User.Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(int articleId)
        {
            var result = await _articleService.Delete(articleId, User.Identity.Name);
            var articleResult = JsonSerializer.Serialize(result);
            return Json(articleResult);
        }

        [Authorize(Roles = "SuperAdmin,User.Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            var result = await _articleService.GetAllByNonDeletedByActive();
            var articlesResult = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                ReferenceHandler=ReferenceHandler.IgnoreCycles
            });
            return Json(articlesResult);
        }



        [Authorize(Roles = "SuperAdmin,Article.Read")]
        [HttpGet]
        public async Task<IActionResult> DeletedArticles()
        {
            var result = await _articleService.GetAllByDeletedAsync();
            if (result.ResultStatus == ResultStatusType.Success)
            {
                return View(result.Data);
            }
            return NotFound();
        }

        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpPost]
        public async Task<IActionResult> UndoDelete(int articleId)
        {
            var result = await _articleService.UndoDeleteAsync(articleId, User.Identity.Name);
            var articleResult = JsonSerializer.Serialize(result);
            return Json(articleResult);
        }

        [Authorize(Roles = "SuperAdmin,User.Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllDeletedArticles()
        {
            var result = await _articleService.GetAllByDeletedAsync();
            var articlesResult = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
            return Json(articlesResult);
        }


        [Authorize(Roles = "SuperAdmin,User.Delete")]
        [HttpPost]
        public async Task<IActionResult> HardDelete(int articleId)
        {
            var result = await _articleService.HardDelete(articleId);
            var articleResult = JsonSerializer.Serialize(result);
            return Json(articleResult);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllByViewCount(bool isAscending,int takeSize)
        {
            var result = await _articleService.GetAllViewCountAsync(isAscending,takeSize);
            var articlesResult = JsonSerializer.Serialize(result.Data.Articles, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(articlesResult);
        }
    }
}
