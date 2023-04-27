using AutoMapper;
using Domain.Models;
using Infrastructure.Services;

namespace Infrastructure.Profiles;

public class MempoolProfile : Profile
{
    public MempoolProfile()
    {
        CreateMap<MempoolBlock, Block>().ForMember(
            b => b.TimeStamp,
            opt => opt.MapFrom(
                (mb, b) => DateTime.MinValue
            )
        );
    }
}