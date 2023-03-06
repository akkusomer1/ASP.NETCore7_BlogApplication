using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NToastNotify;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Helpers.Abstract;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OptionsController : Controller
    {
        //AboutSettings
        private readonly AboutUsPageInfo _aboutPageInfo;
        private readonly IWritableOptions<AboutUsPageInfo> _aboutUsPageInfoOptions;

        //GeneralSettings
        private readonly WebSiteInfo _webSiteInfo;
        private readonly IWritableOptions<WebSiteInfo> _webSiteInfoOptions;

        //EmailSettings
        private readonly SmtpSettings _smtpSettings;
        private readonly IWritableOptions<SmtpSettings> _smtpSettingsOptions;
        //Tostr
        private readonly IToastNotification _notification;

        //ArticleRightSideBarWidgetOptions
        private readonly ArticleRightSideBarWidgetOptions _articleRightSideBarWidgetOptions;
        private readonly IWritableOptions<ArticleRightSideBarWidgetOptions> _sideBarWidgetWritableOptions;

        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        //constr. 
        public OptionsController(
            IOptionsSnapshot<AboutUsPageInfo> aboutPageInfo,
            IWritableOptions<AboutUsPageInfo> aboutUsPageInfoOptions,
            IToastNotification notification,
            IOptionsSnapshot<WebSiteInfo> webSiteInfo,
            IWritableOptions<WebSiteInfo> webSiteInfoOptions,
            IOptionsSnapshot<SmtpSettings> smtpSettings,
            IWritableOptions<SmtpSettings> smtpSettingsOptions, IOptionsSnapshot<ArticleRightSideBarWidgetOptions> articleRightSideBarWidgetOptions, IWritableOptions<ArticleRightSideBarWidgetOptions> sideBarWidgetWritableOptions, ICategoryService categoryService, IMapper mapper)
        {
            _aboutUsPageInfoOptions = aboutUsPageInfoOptions;
            _notification = notification;
            _webSiteInfo = webSiteInfo.Value;
            _webSiteInfoOptions = webSiteInfoOptions;
            _smtpSettings = smtpSettings.Value;
            _smtpSettingsOptions = smtpSettingsOptions;
            _sideBarWidgetWritableOptions = sideBarWidgetWritableOptions;
            _categoryService = categoryService;
            _mapper = mapper;
            _articleRightSideBarWidgetOptions = articleRightSideBarWidgetOptions.Value;
            _aboutPageInfo = aboutPageInfo.Value;
        }


        [HttpGet]
        public async Task<IActionResult> ArticleRightSideBarWidgetSettings()
        {
            var categories = await _categoryService.GetAllByNonDeleted();
            var vm = _mapper.Map<ArticleRightSideBarWidgetOptionsViewModel>(_articleRightSideBarWidgetOptions);
            vm.Categories = categories.Data.Categories;
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> ArticleRightSideBarWidgetSettings(ArticleRightSideBarWidgetOptionsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _sideBarWidgetWritableOptions.Update(x =>
                {
                    x.Header = vm.Header;
                    x.TakeSize = vm.TakeSize;
                    x.CategoryId = vm.CategoryId;
                    x.FilterBy = vm.FilterBy;
                    x.OrderBy = vm.OrderBy;
                    x.IsAscending = vm.IsAscending;
                    x.StartAt = vm.StartAt;
                    x.EndAt = vm.EndAt;
                    x.MaxViewCount = vm.MaxViewCount;
                    x.MinViewCount = vm.MinViewCount;
                    x.MaxCommentCount = vm.MaxCommentCount;
                    x.MinCommentCount = vm.MinCommentCount;
                });
                _notification.AddSuccessToastMessage("Makale sayfalarınızın widget ayarları başarıyla güncellenmiştir", new ToastrOptions
                {
                    Title = "Başarılı İşlem"
                });
                var categoryList = await _categoryService.GetAllByNonDeleted();
                vm.Categories = categoryList.Data.Categories;
                return View(vm);
            }
            var categories = await _categoryService.GetAllByNonDeleted();
            vm.Categories = categories.Data.Categories;
            return View(vm);

        }





        [HttpGet]
        public IActionResult GeneralSettings()
        {
            return View(_webSiteInfo);
        }

        [HttpPost]
        public IActionResult GeneralSettings(WebSiteInfo webSiteInfo)
        {
            if (ModelState.IsValid)
            {
                _webSiteInfoOptions.Update(x =>
                {
                    x.Title = webSiteInfo.Title;
                    x.MenuTitle = webSiteInfo.MenuTitle;
                    x.SeoAuthor = webSiteInfo.SeoAuthor;
                    x.SeoDescription = webSiteInfo.SeoDescription;
                    x.SeoTags = webSiteInfo.SeoTags;
                });
                _notification.AddSuccessToastMessage("Hakkımızda sayfa içerikleri başarıyla güncellenmiştir", new ToastrOptions
                {
                    Title = "Başarılı İşlem"
                });
                return View(webSiteInfo);
            }
            return View(webSiteInfo);

        }

        [HttpGet]
        public IActionResult EmailSettings()
        {
            return View(_smtpSettings);
        }

        [HttpPost]
        public IActionResult EmailSettings(SmtpSettings smtpSettings)
        {
            if (ModelState.IsValid)
            {
                _smtpSettingsOptions.Update(x =>
                {
                    x.Server = smtpSettings.Server;
                    x.Port = smtpSettings.Port;
                    x.SenderEmail = smtpSettings.SenderEmail;
                    x.Username = smtpSettings.Username;
                    x.Password = smtpSettings.Password;
                });
                _notification.AddSuccessToastMessage("E-Posta ayarlar başarıyla güncellenmiştir", new ToastrOptions
                {
                    Title = "Başarılı İşlem"
                });
                return View(smtpSettings);
            }
            return View(smtpSettings);
        }


        [HttpGet]
        public IActionResult About()
        {
            return View(_aboutPageInfo);
        }

        [HttpPost]
        public IActionResult About(AboutUsPageInfo aboutUsPageInfo)
        {
            if (ModelState.IsValid)
            {
                _aboutUsPageInfoOptions.Update(x =>
                {
                    x.Header = aboutUsPageInfo.Header;
                    x.Content = aboutUsPageInfo.Content;
                    x.SeoAuthor = aboutUsPageInfo.SeoAuthor;
                    x.SeoDescription = aboutUsPageInfo.SeoDescription;
                    x.SeoTags = aboutUsPageInfo.SeoTags;
                });

                _notification.AddSuccessToastMessage("Hakkımızda sayfa içerikleri başarıyla güncellenmiştir", new ToastrOptions
                {
                    Title = "Başarılı İşlem"
                });
                return View(aboutUsPageInfo);
            }

            return View(aboutUsPageInfo);

        }
    }
}
