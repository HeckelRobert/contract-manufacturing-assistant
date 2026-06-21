namespace QuotationAccelerator.SharedKernel.Results;

public sealed class Result
{
    private Result(bool isSuccess, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public IReadOnlyList<string> Errors { get; }

    public static Result Success() => new(true, []);

    public static Result Failure(params string[] errors) => new(false, errors);

    public static Result Failure(IEnumerable<string> errors) => new(false, errors.ToList());
}

public sealed class Result<T>
    where T : notnull
{
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Errors = [];
    }

    private Result(IReadOnlyList<string> errors)
    {
        IsSuccess = false;
        Value = default;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public IReadOnlyList<string> Errors { get; }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(params string[] errors) => new(errors);

    public static Result<T> Failure(IEnumerable<string> errors) => new(errors.ToList());
}
