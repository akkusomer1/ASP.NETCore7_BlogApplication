using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Dtos;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Mvc.Models;
using ProgrammersBlog.Services.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NToastNotify;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Mvc.Helpers.Abstract;

namespace ProgrammersBlog.Mvc.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IWritableOptions<AboutUsPageInfo> _aboutUsPageInfoOptions;
        private readonly IArticleService _articleService;
        private readonly AboutUsPageInfo _aboutUsPageInfo;
        private  readonly  IMailService _mailService;
        private readonly IToastNotification _toastNotification;

      
        public HomeController(IArticleService articleService, IOptionsSnapshot<AboutUsPageInfo> aboutUsPageInfo, IMailService mailService, IToastNotification toastNotification, IWritableOptions<AboutUsPageInfo> aboutUsPageInfoOptions)
        {
            _articleService = articleService;
            _mailService = mailService;
            _toastNotification = toastNotification;
            _aboutUsPageInfoOptions = aboutUsPageInfoOptions;
        
            _aboutUsPageInfo = aboutUsPageInfo.Value;
        }

        [Route("Hakkimizda")]
        [HttpGet]
        public async Task<IActionResult> About()
        {
            return View(_aboutUsPageInfo);
        }

        [Route("Anasayfa/{currentPage}/{pageSize}/{isAscending}")]
        [Route("Anasayfa/{categoryId}/{currentPage}/{pageSize}/{isAscending}")]
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            var articles = await (categoryId == null
                ? _articleService.GetAllByPagingAsync(null, currentPage, pageSize, isAscending)
                : _articleService.GetAllByPagingAsync(categoryId.Value, currentPage, pageSize, isAscending));
            return View(articles.Data);
        }

       
        [Route("iletişim")]
        [Route("ileti")]
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            return View();
        }


        [Route("iletişim")]
        [Route("ileti")]
        [HttpPost]
        public async Task<IActionResult> Contact(EmailSendDto emailSendDto)
        {
            if (ModelState.IsValid)
            {
               var result= _mailService.SendContactEmail(emailSendDto);

               if (result.ResultStatus == ResultStatusType.Success)
               {
                   _toastNotification.AddSuccessToastMessage(result.Message,new ToastrOptions
                   {
                       Title = "Başarılı İşlem"
                   });
                   return View();
               }
            }
            return View(emailSendDto);
        }

    }
}