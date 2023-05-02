using AutoMapper;
using Domain.Models;
using Service.Models.Requests;

namespace Service.Profiles;

public class RequestProfile:Profile
{
    public RequestProfile()
    {
        CreateMap<SessionActionDto, SessionAction>();
    }
}