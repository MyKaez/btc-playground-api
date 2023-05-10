namespace Application.Extensions;

public static class StringExtensions
{
    public static string JoinToString(this IEnumerable<object> enumerable, string separator = ",") =>
        string.Join(separator, enumerable);

    public static string JoinToString<T, T2>(this IEnumerable<KeyValuePair<T, T2>> enumerable, string separator = ",") =>
        string.Join(separator, enumerable.Select(e => $"{e.Key}={e.Value}"));
}