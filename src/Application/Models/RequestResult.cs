namespace Application.Models;

public class RequestResult<TValue, TError>
{
    private RequestResult(TError error)
    {
        Error = error;
        IsValid = false;
    }

    private RequestResult(TValue value)
    {
        Value = value;
        IsValid = true;
    }

    public TValue? Value { get; }

    public TError? Error { get; }

    public bool IsValid { get; }

    public static implicit operator RequestResult<TValue, TError>(TValue value) => new(value);

    public static implicit operator RequestResult<TValue, TError>(TError error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> error)
    {
        return IsValid ? success(Value!) : error(Error!);
    }
}