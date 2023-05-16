using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using Domain.Models;

namespace Infrastructure.Profiles;

public class DatabaseProfile : Profile
{
    public DatabaseProfile()
    {
        CreateMap<Database.Session, Session>()
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
        CreateMap<Database.User, User>();
    }
}