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
                opt => opt.MapFrom((p, pd) => p.CurrentPrice)
            );
    }
}