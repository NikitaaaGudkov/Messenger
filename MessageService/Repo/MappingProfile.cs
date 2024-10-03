using AutoMapper;
using MessageService.Db;
using MessageService.DTO;

namespace MessageService.Repo
{
    /// <summary>
    /// Класс, преобразующий сущность из базы данных в сущность бизнес-логики и наоборот
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderId, opts => opts.MapFrom(y => y.SenderId))
                .ForMember(dest => dest.ConsumerId, opts => opts.MapFrom(y => y.ConsumerId))
                .ForMember(dest => dest.Text, opts => opts.MapFrom(y => y.Text))
                .ForMember(dest => dest.DateTime, opts => opts.MapFrom(y => y.DateTime))
                .ReverseMap();
        }
    }
}
