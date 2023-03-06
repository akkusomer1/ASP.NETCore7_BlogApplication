using AutoMapper;
using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;
using ProgrammersBlog.Core.Concrete.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Mapping
{
    public class ArticleMapProfile:Profile
    {
        public ArticleMapProfile()
        {
            CreateMap<Article, ArticleDto>().ReverseMap();
            CreateMap<Article, ArticleListDto>().ReverseMap();
            CreateMap<Article, ArticleCreateDto>().ReverseMap();
            CreateMap<Article, ArticleUpdateDto>().ReverseMap();
            CreateMap<Article, ArticleUpdateDto>().ReverseMap();
        }
    }
}
