using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

public class Result<TValue>
{
    private Result(TValue value)
    {
        Value = value;
        IsValid = true;
    }

    public Result(IRequestError error)
    {
        Error = error;
        IsValid = false;
    }

    private TValue? Value { get; }

    private IRequestError? Error { get; }

    private bool IsValid { get; }

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(NotFoundResult error) => new(error);

    public static implicit operator Result<TValue>(NotAuthorizedResult error) => new(error);

    public static implicit operator Result<TValue>(BadRequest error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> success, Func<IRequestError, TResult> error)
    {
        return IsValid ? success(Value!) : error(Error!);
    }

    public bool TryGetValue([NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out IRequestError? error)
    {
        value = Value;
        error = Error;
        return IsValid;
    }
}