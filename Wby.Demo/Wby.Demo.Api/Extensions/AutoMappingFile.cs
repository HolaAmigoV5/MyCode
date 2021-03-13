using AutoMapper.Configuration;
using Wby.Demo.Shared.DataModel;
using Wby.Demo.Shared.Dto;

namespace Wby.Demo.Api.Extensions
{
    public class AutoMappingFile : MapperConfigurationExpression
    {
        public AutoMappingFile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Group, GroupDto>().ReverseMap();
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Basic, BasicDto>().ReverseMap();

            CreateMap<GroupUserDto, GroupUser>().ReverseMap();
            CreateMap<GroupUser, GroupUserDto>().ReverseMap();
        }
    }
}
