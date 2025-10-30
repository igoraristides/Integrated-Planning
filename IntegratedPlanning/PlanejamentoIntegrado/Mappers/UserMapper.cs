using AutoMapper;
using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserViewModel>();

        CreateMap<UserViewModel, User>()
            .ForMember(
                dest => dest.IsActive,
                opt => opt.MapFrom(src => !src.IsActive.HasValue || src.IsActive.Value ? 1 : 0)
            );

    }
}
