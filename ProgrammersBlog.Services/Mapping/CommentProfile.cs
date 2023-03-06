using AutoMapper;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Mapping
{
    public class CommentProfile:Profile
    {
        public CommentProfile()
        {
            CreateMap<CommentAddDto, Comment>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(x => DateTime.Now))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(x => DateTime.Now))
                .ForMember(dest => dest.UpdateByName, opt => opt.MapFrom(x => x.CreatedByName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(x => false));
        
            CreateMap<CommentUpdateDto, Comment>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(x => DateTime.Now));
          
            CreateMap<Comment, CommentUpdateDto>();
            CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}
