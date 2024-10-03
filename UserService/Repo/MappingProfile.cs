using AutoMapper;
using UserService.AuthorizationModel;
using UserService.Db;

namespace UserService.Repo
{
    /// <summary>
    /// Класс, преобразующий сущность из базы данных в сущность бизнес-логики и наоборот
    /// </summary>
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
