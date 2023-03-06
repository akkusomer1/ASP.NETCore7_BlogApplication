using AutoMapper;
using ProgrammersBlog.Core.Concrete.Dtos.UserDto;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Services.Mapping
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<UserAddDto, User>();
            CreateMap<User, UserUpdateDto>().ReverseMap();
        }
    }
}
