using AutoMapper;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserViewModel>()
            .ForMember(
                dest => dest.ReceivesEmail,
                opt => opt.MapFrom(src => src.ReceivesEmail == 1)
            );

        CreateMap<UserViewModel, User>()
            .ForMember(
                dest => dest.IsActive,
                opt => opt.MapFrom(src => !src.IsActive.HasValue || src.IsActive.Value ? 1 : 0)
            )
            .ForMember(
                dest => dest.ReceivesEmail,
                opt => opt.MapFrom(src => src.ReceivesEmail ? 1 : 0)
            );
    }
}
