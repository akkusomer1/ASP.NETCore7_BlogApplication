using AutoMapper;
using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Mapping
{
    public class CategoryMapProfile:Profile
    {
        public CategoryMapProfile()
        {
            CreateMap<Category,CategoryAddDto>().ReverseMap();
            CreateMap<Category,CategoryUpdateDto>().ReverseMap();
            CreateMap<Category,CategoryListDto>().ReverseMap();
        }
    }
}
