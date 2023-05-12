using AutoMapper;
using Domain.Models;
using Service.Models;

namespace Service.Profiles;

public class ResponseProfile : Profile
{
    public ResponseProfile()
    {
        CreateMap<Block, BlockDto>();
        CreateMap<Price, PriceDto>()
            .ForMember(
                p => p.Price,
                opt => opt.MapFrom(p => p.CurrentPrice)
            );
        CreateMap<Session, SessionDto>()
            .ForMember(p => p.ExpirationTime,
                opt => opt.MapFrom(p => DateTime.Now.Add(p.ExpiresIn)));
        CreateMap<Session, SessionControlDto>()
            .ForMember(p => p.ExpirationTime,
                opt => opt.MapFrom(p => DateTime.Now.Add(p.ExpiresIn)));
        CreateMap<SessionStatus, SessionStatusDto>();
        CreateMap<SessionSuggestion, SessionSuggestionDto>();
        CreateMap<User, UserDto>();
        CreateMap<UserSuggestion, UserSuggestionDto>();
    }
}