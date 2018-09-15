using System;

namespace Feedz.Util
{
    public interface IResult
    {
        ResultType Type { get; }
        bool Successful { get; }
        bool Failure { get; }
        string Message { get; }
        Result<T> FailureAs<T>();
    }

    public enum ResultType
    {
        Success,
        Error,
        Forbidden,
        Invalid,
        NotFound
    }

    public class Result : IResult
    {
        protected Result()
        {
        }

        public ResultType Type { get; protected set; }
        public bool Successful => Type == ResultType.Success;
        public bool Failure => Type != ResultType.Success;

        public string Message { get; protected set; }

        public Result<T> FailureAs<T>()
            => Result<T>.FromFailure(this);

        public static Result Success()
            => new Result()
            {
                Type = ResultType.Success
            };


        public static Result Error(string message)
            => new Result()
            {
                Type = ResultType.Error,
                Message = message
            };


        public static Result Forbidden(string message)
            => new Result()
            {
                Type = ResultType.Forbidden,
                Message = message
            };


        public static Result Invalid(string message)
            => new Result()
            {
                Type = ResultType.Invalid,
                Message = message
            };

        public static Result<T> Error<T>(string error)
            => Result<T>.Error(error);

        public static Result<T> Forbidden<T>(string error)
            => Result<T>.Forbidden(error);

        public static Result<T> Invalid<T>(string error)
            => Result<T>.Invalid(error);

        public static Result<T> NotFound<T>(string error)
            => Result<T>.NotFound(error);

        public static Result DiscardValue<T>(Result<T> result)
            => new Result
            {
                Message = result.Message,
                Type = result.Type
            };

    }

    public class Result<T> : IResult
    {
        private T _value;

        private Result()
        {
        }

        public ResultType Type { get; protected set; }
        public bool Successful => Type == ResultType.Success;
        public bool Failure => Type != ResultType.Success;

        public string Message { get; protected set; }

        public T Value
        {
            get
            {
                if (!Successful)
                    throw new Exception("Operation was not successful");
                return _value;
            }
        }

        public Result<TOther> FailureAs<TOther>()
            => Result<TOther>.FromFailure(this);


        public static Result<T> FromFailure(IResult result)
        {
            if(result.Successful)
                throw new InvalidOperationException("Result was not a failure");

            return new Result<T>()
            {
                Type = result.Type,
                Message = result.Message
            };
        }

        public static Result<T> Success(T value)
            => new Result<T>()
            {
                _value = value,
                Type = ResultType.Success
            };

        public static Result<T> Error(string message)
            => new Result<T>()
            {
                Type = ResultType.Error,
                Message = message
            };

        public static Result<T> Forbidden(string message)
            => new Result<T>()
            {
                Type = ResultType.Forbidden,
                Message = message
            };

        public static Result<T> Invalid(string message)
            => new Result<T>()
            {
                Type = ResultType.Invalid,
                Message = message
            };

        public static Result<T> NotFound(string message) => new Result<T>()
        {
            Type = ResultType.NotFound,
            Message = message
        };
    }

    public static class ResultExtensions
    {
        public static Result<T> AsResult<T>(this T value)
            => Result<T>.Success(value);
    }
}