using AutoMapper;
using UserManager.DAL.Models;
using UserManager.DTO.Tags;
using UserManager.DTO.Users;

namespace UserManager.Web.Api.Configs
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<UsersTags, UserTagDTO>().ReverseMap();
            CreateMap<Users, UserDTO>().ReverseMap();
            CreateMap<Tags, TagDTO>().ReverseMap();
        }
    }
}
