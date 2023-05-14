using System.Security.Cryptography;
using System.Text;

namespace Application.Extensions;

public static class StringExtensions
{
    public static string JoinToString(this IEnumerable<object> enumerable, string separator = ",") =>
        string.Join(separator, enumerable);

    public static string JoinToString<T, T2>(this IEnumerable<KeyValuePair<T, T2>> enumerable, string separator = ",") =>
        string.Join(separator, enumerable.Select(e => $"{e.Key}={e.Value}"));
    
    public static string ComputeSha256Hash(this string rawData)
    {
        var buffer = Encoding.UTF8.GetBytes(rawData);
        var bytes = SHA256.HashData(buffer);
        var builder = new StringBuilder();

        foreach (var @byte in bytes)
            builder.Append(@byte.ToString("x2"));

        return builder.ToString();
    }
}