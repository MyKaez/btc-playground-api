using AutoMapper;
using Domain.Models;

namespace Infrastructure.Profiles;

public class DatabaseProfile:Profile
{
    public DatabaseProfile()
    {
        CreateMap<Database.Session, Session>();
        CreateMap<Database.User, User>();
    }
}