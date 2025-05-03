namespace Auctions.Application;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T? value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, null);
    public new static Result<T> Failure(string error) => new(default, false, error);
}

public class PaginatedResult<T> : Result<T>
{
    public string? NextCursor { get; }
    public string? PreviousCursor { get; }
    
    private PaginatedResult(
        T? value, bool isSuccess, string? error, string? nextCursor, string? previousCursor)
        : base(value, isSuccess, error)
    {
        NextCursor = nextCursor;
        PreviousCursor = previousCursor;
    }
    
    public static PaginatedResult<T> Success(T value, string? nextCursor, string? previousCursor) => 
        new(value, true, null, nextCursor, previousCursor);
    
    public new static PaginatedResult<T> Failure(string error) => new(default, false, error, null, null);
}
