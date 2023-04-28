using AutoMapper;
using Domain.Models;
using Infrastructure.Services;

namespace Infrastructure.Profiles;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<MempoolBlock, Block>()
            .ForMember(
                b => b.TimeStamp,
                opt => opt.MapFrom(
                    (mb, b) => DateTimeOffset.FromUnixTimeSeconds(mb.TimeStamp).DateTime
                )
            );

        CreateMap<BlockchainInfoPrice, Price>()
            .ForMember(
                p => p.Currency,
                opt => opt.MapFrom(
                    (bip, p) => bip.Currency)
            ).ForMember(
                p => p.CurrentPrice,
                opt => opt.MapFrom(
                    (bip, p) => bip.Last)
            ).ForMember(
                p => p.PreviousPrice,
                opt => opt.MapFrom(
                    (bip, p) => bip._15M)
            );
    }
}