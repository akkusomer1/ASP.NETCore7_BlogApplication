using Microsoft.AspNetCore.Mvc.Filters;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Filter
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class ViewCountFilterAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var articleId = context.ActionArguments["articleId"];

            if (articleId is not null)
            {
                string articleValue = context.HttpContext.Request.Cookies[$"article{articleId}"];

                if (string.IsNullOrEmpty(articleValue))
                {
                    Set($"article{articleId}", articleId.ToString(), 1, context.HttpContext.Response);

                    var articleService = context.HttpContext.RequestServices.GetService<IArticleService>();

                   await articleService.IncreaseViewCountAsync(Convert.ToInt32(articleId));

                    await next();
                }

            }
            await next();
        }

        public void Set(string key, string value, int? expireTime, HttpResponse response)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddYears(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMonths(6);

            response.Cookies.Append(key, value, option);
        }

        public void Remove(string key, HttpResponse response)
        {
            response.Cookies.Delete(key);
        }
    }
}

