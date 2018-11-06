using AutoMapper;
using Moniter.Features.Alert;
using Moniter.Features.Parser;
using Moniter.Features.Users;

namespace Moniter.Features
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.User, User>(MemberList.None);
            CreateMap<Models.User, LoginUser>(MemberList.None);
            CreateMap<AlertInfo, Save.Command>(MemberList.None);
        }
    }
}