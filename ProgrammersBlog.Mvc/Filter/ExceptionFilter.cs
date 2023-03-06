using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Filter
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _environment;
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly ILogger _logger;
        public ExceptionFilter(IHostEnvironment environment, IModelMetadataProvider metadataProvider, ILogger<ExceptionFilter> logger)
        {
            _environment = environment;
            _metadataProvider = metadataProvider;
            _logger = logger;
        }


        public void OnException(ExceptionContext context)
        {
            if (_environment.IsDevelopment())
            {
                context.ExceptionHandled = true;

                ErrorModel errorModel = new ErrorModel();
                ViewResult result;
                switch (context.Exception)
                {
                    case SqlNullValueException:
                        errorModel.Message =
                            "Üzgünüz, işleminiz sırasında beklenmedik bir veritabanı hatası oluştu. Sorunu en kısa sürede çözeceğiz.";
                        errorModel.Detail = context.Exception.Message;
                        result = new ViewResult { ViewName = "Error" };
                        result.StatusCode = 500;
                        errorModel.StatusCode = 500;
                        _logger.LogError(context.Exception, context.Exception.Message);
                        break;

                    case NullReferenceException:
                        errorModel.Message =
                            "Üzgünüz, işleminiz sırasında beklenmedik bir null veriye rastlandı. Sorunu en kısa sürede çözeceğiz.";
                        errorModel.Detail = context.Exception.Message;
                        result = new ViewResult { ViewName = "Error" };
                        result.StatusCode = 400;
                        errorModel.StatusCode = 400;
                        _logger.LogError(context.Exception, context.Exception.Message);
                        break;

                    default:
                        errorModel.Message =
                            "Üzgünüz, işleminiz sırasında beklenmedik bir hata oluştu. Sorunu en kısa sürede çözeceğiz.";
                        result = new ViewResult { ViewName = "Error" };
                        errorModel.Detail = context.Exception.Message;
                        result.StatusCode = 500;
                        errorModel.StatusCode = 500;
                        _logger.LogError(context.Exception, "Bu benim log hata mesajım");
                        break;

                }
                result.ViewData = new ViewDataDictionary(_metadataProvider, context.ModelState);
                result.ViewData.Add("MvcErrorModel", errorModel);

                context.Result = result;
            }
        }
    }
}
