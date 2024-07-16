using MessagePack;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Primitives;

/// <summary>
/// Describes the success or failure of any kind of operation.
/// </summary>
[DataContract]
public class Result
{
    /// <summary>
    /// For serialization only.
    /// </summary>
    [SerializationConstructor]
    [JsonConstructor]
    public Result() { }

    public Result(bool isSuccess, string reason = "", Exception? exception = null)
    {
        if (!isSuccess && exception is null && string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("A reason or exception must be supplied for an unsuccessful result.");
        }

        IsSuccess = isSuccess;
        Exception = exception;
        Reason = reason;
    }

    [IgnoreDataMember]
    public Exception? Exception { get; init; }

    [IgnoreDataMember]
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool HadException => Exception is not null;

    [DataMember]
    public bool IsSuccess { get; init; }

    [DataMember]
    public string Reason { get; init; } = string.Empty;


    public static Result Fail(string reason)
    {
        return new Result(false, reason);
    }

    public static Result Fail(Exception ex)
    {
        return new Result(false, string.Empty, ex);
    }

    public static Result<T> Fail<T>(string reason)
    {
        return new Result<T>(reason);
    }

    public static Result<T> Fail<T>(Exception ex)
    {
        return new Result<T>(ex);
    }

    public static Result Ok()
    {
        return new Result(true);
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value);
    }
}


/// <summary>
/// Describes the success or failure of any kind of operation.
/// </summary>
[DataContract]
public class Result<T>
{
    /// <summary>
    /// Returns a successful result with the given value.
    /// </summary>
    /// <param name="value"></param>
    public Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    /// <summary>
    /// Returns an unsuccessful result with the given exception.
    /// </summary>
    /// <param name="exception"></param>
    public Result(Exception exception)
    {
        IsSuccess = false;
        Exception = exception;
        Reason = exception.Message;
    }

    /// <summary>
    /// Returns an unsuccessful result with the given reason.
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <exception cref="ArgumentException"></exception>
    public Result(string reason)
    {
        IsSuccess = false;
        Reason = reason;
    }

    /// <summary>
    /// For serialization only.
    /// </summary>
    [SerializationConstructor]
    [JsonConstructor]
    public Result() { }

    public Result(Exception? exception, bool isSuccess, string reason, T? value)
    {
        Exception = exception;
        IsSuccess = isSuccess;
        Reason = reason;
        Value = value;
    }

    [IgnoreDataMember]
    public Exception? Exception { get; init; }

    [IgnoreDataMember]
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool HadException => Exception is not null;

    [DataMember]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; init; }

    [DataMember]
    public string Reason { get; init; } = string.Empty;

    [DataMember]
    public T? Value { get; init; }
}
