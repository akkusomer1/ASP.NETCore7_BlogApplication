using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Mvc.Extantions;
using ProgrammersBlog.Mvc.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProgrammersBlog.Mvc.Controllers
{
    //HTttpost AddComment metodumu ekliyorum.
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<JsonResult> Add(CommentAddDto commentAddDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _commentService.AddAsync(commentAddDto);
                if (result.ResultStatus == ResultStatusType.Success)
                {
                    var jsonAjaxModel = JsonSerializer.Serialize(new CommentAddAjaxViewModel
                    {
                        CommentDto = result.Data,
                        CommentAddPartial = await this.RenderViewToStringAsync("_CommentAddPartial",commentAddDto)
                    },new JsonSerializerOptions { ReferenceHandler= ReferenceHandler .Preserve});
                    return Json(jsonAjaxModel);
                }
                ModelState.AddModelError("", result.Message);
            }
            var jsonErrorAjaxModel = JsonSerializer.Serialize(new CommentAddAjaxViewModel
            {
                CommentAddDto = commentAddDto,
                CommentAddPartial = await this.RenderViewToStringAsync("_CommentAddPartial", commentAddDto)
            }, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            return Json(jsonErrorAjaxModel);
        }
    }
}
