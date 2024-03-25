using AutoMapper;
using MessageLibrary;
using MessageService.Db;

namespace MessageService.Repo
{
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
