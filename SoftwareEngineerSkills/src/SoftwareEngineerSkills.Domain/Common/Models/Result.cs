namespace SoftwareEngineerSkills.Domain.Common.Models;

/// <summary>
/// Interface for result objects that represents the outcome of an operation
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    bool IsSuccess { get; }
    
    /// <summary>
    /// Gets the error message if the operation failed
    /// </summary>
    string? Error { get; }
}

/// <summary>
/// Represents the result of an operation with a return value
/// </summary>
/// <typeparam name="T">Type of the result value</typeparam>
public class Result<T> : IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Gets the error message if the operation failed
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// Gets the result value if the operation was successful
    /// </summary>
    public T? Value { get; }
    
    /// <summary>
    /// Creates a success result with the specified value
    /// </summary>
    /// <param name="value">The result value</param>
    /// <returns>A success result</returns>
    public static Result<T> Success(T value) => new Result<T>(true, value, null);
    
    /// <summary>
    /// Creates a failure result with the specified error message
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failure result</returns>
    public static Result<T> Failure(string error) => new Result<T>(false, default, error);
    
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
}

/// <summary>
/// Represents the result of an operation without a return value
/// </summary>
public class Result : IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Gets the error message if the operation failed
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// Creates a success result
    /// </summary>
    /// <returns>A success result</returns>
    public static Result Success() => new Result(true, null);
    
    /// <summary>
    /// Creates a failure result with the specified error message
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failure result</returns>
    public static Result Failure(string error) => new Result(false, error);
    
    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}
