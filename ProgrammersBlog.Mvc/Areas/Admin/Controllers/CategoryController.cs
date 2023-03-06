using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;
using ProgrammersBlog.Core.Result.Concrete;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Extantions;
using ProgrammersBlog.Services.Services;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [Authorize(Roles = "SuperAdmin,Category.Read")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            IDataResult<CategoryListDto> result = await _service.GetAllByNonDeleted();

            CategoryListDto categoryListDto = result.Data;

            CategoryCreateAndListDto dto = new CategoryCreateAndListDto()
            {
                CategoryListDto = categoryListDto,
                CategoryAddDto = new CategoryAddDto()
            };
            return View(dto);
        }

        [Authorize(Roles = "SuperAdmin,Category.Add")]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("CategoryAddPartial");
        }


        [Authorize(Roles = "SuperAdmin,Category.Add")]
        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddDto categoryAddDto)
        {

            if (ModelState.IsValid)
            {
                var result = await _service.Add(categoryAddDto, User.Identity.Name);
                if (result.ResultStatus == ResultStatusType.Success)
                {
                    var categoryAjaxViewModal = new CategoryAjaxViewModal
                    {
                        CategoryDto = result.Data,
                        CategoryAddPartial = await this.RenderViewToStringAsync("CategoryAddPartial", categoryAddDto)
                    };
                    var jsonCategoryAjaxViewModal = JsonSerializer.Serialize(categoryAjaxViewModal);
                    return Json(jsonCategoryAjaxViewModal);
                }
            }
            var categoryAjaxErrorViewModal = JsonSerializer.Serialize(new CategoryAjaxViewModal()
            {
                CategoryAddPartial = await this.RenderViewToStringAsync("CategoryAddPartial", categoryAddDto)
            });
            return Json(categoryAjaxErrorViewModal);
        }

        [Authorize(Roles = "SuperAdmin,Category.Read")]
        [HttpGet]
        public async Task<JsonResult> GetAllCategories()
        {
            var categories = await _service.GetAllByNonDeleted();
            var jsonCategories = JsonSerializer.Serialize(categories.Data, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(jsonCategories);
        }

        [Authorize(Roles = "SuperAdmin,Category.Delete")]
        [HttpPost]
        public async Task<JsonResult> Delete(int categoryId)
        {
            var result = await _service.Delete(categoryId, User.Identity.Name);
            var jsonResult = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });

            return Json(jsonResult);
        }

        [Authorize(Roles = "SuperAdmin,Category.Update")]
        [HttpGet]
        public async Task<IActionResult> Update(int categoryId)
        {
            var result = await _service.GetCategoryUpdateDto(categoryId);

            if (result.ResultStatus==ResultStatusType.Success)
            {
                return PartialView("CategoryUpdatePartial", result.Data);
            }
            return NotFound();
        }

        [Authorize(Roles = "SuperAdmin,Category.Update")]
        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateDto categoryUpdateDto)
        {

            if (ModelState.IsValid)
            {
                var result = await _service.Update(categoryUpdateDto, "Ömer Akkuş");
                if (result.ResultStatus == ResultStatusType.Success)
                {
                    var categoryUpdateAjaxViewModal = new CategoryUpdateAjaxViewModal
                    {
                        CategoryDto = result.Data,
                        CategoryUpdatePartial = await this.RenderViewToStringAsync("CategoryUpdatePartial", categoryUpdateDto)
                    };
                    var jsonCategoryAjaxViewModal = JsonSerializer.Serialize(categoryUpdateAjaxViewModal);
                    return Json(jsonCategoryAjaxViewModal);
                }
            }
            var categoryUpdateAjaxErrorViewModal = JsonSerializer.Serialize(new CategoryUpdateAjaxViewModal()
            {
                CategoryUpdatePartial = await this.RenderViewToStringAsync("CategoryAddPartial", categoryUpdateDto)
            });
            return Json(categoryUpdateAjaxErrorViewModal);
        }


        [Authorize(Roles = "SuperAdmin,Category.Read")]
        [HttpGet]
        public async Task<IActionResult> DeletedCategories()
        {
            IDataResult<CategoryListDto> result = await _service.GetAllByDeleted();  
            return View(result.Data);
        }

        [Authorize(Roles = "SuperAdmin,Category.Read")]
        [HttpGet]
        public async Task<JsonResult> GetAllDeletedCategories()
        {
            var categories = await _service.GetAllByDeleted();
            var jsonCategories = JsonSerializer.Serialize(categories.Data, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(jsonCategories);
        }

         [Authorize(Roles = "SuperAdmin,Category.Update")]
        [HttpPost]
        public async Task<JsonResult> UndoDelete(int categoryId)
        {
            var result = await _service.UndoDelete(categoryId, User.Identity.Name);
            var jsonResult = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });

            return Json(jsonResult);
        }
       
        [Authorize(Roles = "SuperAdmin,Category.Delete")]
        [HttpPost]
        public async Task<JsonResult> HardDelete(int categoryId)
        {
            var result = await _service.HardDelete(categoryId);
            var jsonResult = JsonSerializer.Serialize(result);          
            return Json(jsonResult);
        }
    }
}
