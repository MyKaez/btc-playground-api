using Domain.Models;

namespace Service.Extensions;

public static class SessionExtensions
{
    public const string BlocktrainerSessionName = "Blocktrainer";
    
    public static bool IsDeletable(this Session session)
    {
        return session.Name != BlocktrainerSessionName;
    }
}