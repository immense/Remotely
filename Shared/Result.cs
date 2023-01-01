using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared
{
    public class Result
    {
        public static Result<T> Empty<T>()
        {
            return new Result<T>(true, default);
        }

        public static Result Fail(string error)
        {
            return new Result(false, error);
        }
        public static Result Fail(Exception ex)
        {
            return new Result(false, null, ex);
        }

        public static Result<T> Fail<T>(string error)
        {
            return new Result<T>(false, default, error);
        }

        public static Result<T> Fail<T>(Exception ex)
        {
            return new Result<T>(false, default, exception: ex);
        }

        public static Result Ok()
        {
            return new Result(true);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(true, value, null);
        }


        public Result(bool isSuccess, string error = null, Exception exception = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Exception = exception;

            if (string.IsNullOrWhiteSpace(Error) && !string.IsNullOrWhiteSpace(Exception?.Message))
            {
                Error = Exception.Message;
            }

            Exception ??= new Exception(Error ?? string.Empty);
        }

        public bool IsSuccess { get; init; }

        public string Error { get; init; } = string.Empty;

        public Exception Exception { get; init; }
    }

    public class Result<T>
    {
        public Result(bool isSuccess, T value, string error = null, Exception exception = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Exception = exception;

            if (string.IsNullOrWhiteSpace(Error) && !string.IsNullOrWhiteSpace(Exception?.Message))
            {
                Error = Exception.Message;
            }

            Exception ??= new Exception(Error ?? string.Empty);
        }


        public bool IsSuccess { get; init; }

        public string Error { get; init; } = string.Empty;

        public Exception Exception { get; init; }

        public T Value { get; init; }
    }
}
