using AutoMapper;
using Domain.Models;
using Infrastructure.Mempool;
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
                    mb => DateTimeOffset.FromUnixTimeSeconds(mb.TimeStamp).DateTime)
            );
        CreateMap<MempoolHashRate, HashRateInfo>()
            .ForMember(
                h => h.HashRate,
                opt => opt.MapFrom(
                    hr => hr.CurrentHashRate)
            )
            .ForMember(
                h => h.Difficulty,
                opt => opt.MapFrom(
                    hr => hr.CurrentDifficulty)
            );

        CreateMap<BlockchainInfoPrice, Price>()
            .ForMember(
                p => p.Currency,
                opt => opt.MapFrom(
                    bip => bip.Currency)
            ).ForMember(
                p => p.CurrentPrice,
                opt => opt.MapFrom(
                    bip => bip.Last)
            ).ForMember(
                p => p.PreviousPrice,
                opt => opt.MapFrom(
                    bip => bip._15M)
            );
    }
}