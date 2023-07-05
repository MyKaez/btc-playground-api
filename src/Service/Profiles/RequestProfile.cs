using AutoMapper;
using Domain.Models;
using Infrastructure.Simulations.Models;
using Service.Models.Requests;

namespace Service.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<SessionActionDto, SessionAction>();
        CreateMap<ProofOfWorkRequest, ProofOfWorkSession>();
    }
}