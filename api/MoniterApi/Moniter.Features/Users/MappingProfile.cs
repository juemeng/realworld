using AutoMapper;

namespace Moniter.Features.Users
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.User, User>(MemberList.None);
        }
    }
}