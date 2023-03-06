using AutoMapper;
using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Mvc.Areas.Admin.Models;

namespace ProgrammersBlog.Mvc.Mapping
{
    public class ViewModelMapProfile:Profile
    {
        public ViewModelMapProfile()
        {
            CreateMap< ArticleRightSideBarWidgetOptions, ArticleRightSideBarWidgetOptionsViewModel>();
            CreateMap<ArticleAddViewModel, ArticleCreateDto>();
            CreateMap<ArticleUpdateViewModel, ArticleUpdateDto>().ReverseMap();
        }
    }
}
