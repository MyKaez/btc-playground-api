using System.Text.Json;
using AutoMapper;
using Domain.Models;
using Infrastructure.Database;
using Session = Infrastructure.Database.Session;
using User = Infrastructure.Database.User;

namespace Infrastructure.Profiles;

public class DatabaseProfile : Profile
{
    public DatabaseProfile()
    {
        CreateMap<Session, Domain.Models.Session>()
            .ForMember(
                s => s.Configuration,
                opt => opt.MapFrom(
                    o => o.Configuration != null
                        ? JsonDocument.Parse(o.Configuration, default).RootElement
                        : JsonDocument.Parse("{}", default).RootElement)
            ).ForMember(
                s => s.Status,
                opt => opt.MapFrom(
                    o => Enum.Parse<SessionStatus>(o.Status)
                )
            );
        CreateMap<User, Domain.Models.User>()
            .ForMember(
                u => u.Configuration,
                opt => opt.MapFrom(
                    o => o.Configuration != null
                        ? JsonDocument.Parse(o.Configuration, default).RootElement
                        : JsonDocument.Parse("{}", default).RootElement)
            ).ForMember(
                u => u.Status,
                opt => opt.MapFrom(
                    o => Enum.Parse<UserStatus>(o.Status)
                )
            );
        CreateMap<Connection, Models.Connection>();
    }
}