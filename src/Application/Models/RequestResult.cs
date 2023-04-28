using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

public abstract class RequestResult
{
    public IRequestError? Error { get; init; }

    public bool IsValid { get; protected set; }
    
    public abstract object? Value { get; }
}

public class RequestResult<T> : RequestResult
{
    public RequestResult(IRequestError error)
    {
        Error = error;
        IsValid = false;
    }

    public RequestResult(T result)
    {
        Result = result;
        IsValid = true;
    }

    public T? Result { get; init; }

    public override object? Value => Result;
}