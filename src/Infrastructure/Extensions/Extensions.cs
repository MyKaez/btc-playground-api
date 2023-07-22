namespace Infrastructure.Extensions;

public static class Extensions
{
    public static string UserCacheKey(this Guid sessionId) => $"User_{sessionId}";
}