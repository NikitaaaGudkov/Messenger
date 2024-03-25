using AutoMapper;
using UserService.AuthorizationModel;
using UserService.Db;

namespace UserService.Repo
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserModel>()
                .ForMember(dest => dest.Role, opts => opts.MapFrom(y => y.RoleId))
                .ReverseMap();
        }
    }
}
