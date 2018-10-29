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
        Success = 1,
        Error = 2,
        Forbidden = 3,
        Unauthorised = 4,
        Invalid = 5,
        NotFound = 6,
        Conflict = 7
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


        public static Result Unauthorised(string message)
            => new Result()
            {
                Type = ResultType.Unauthorised,
                Message = message
            };

        public static Result Invalid(string message)
            => new Result()
            {
                Type = ResultType.Invalid,
                Message = message
            };
        
        public static Result Conflict(string message)
            => new Result()
            {
                Type = ResultType.Conflict,
                Message = message
            };

        public static Result<T> Error<T>(string error)
            => Result<T>.Error(error);

        public static Result<T> Forbidden<T>(string error)
            => Result<T>.Forbidden(error);
        
        public static Result<T> Unauthorised<T>(string error)
            => Result<T>.Unauthorised(error);

        public static Result<T> Invalid<T>(string error)
            => Result<T>.Invalid(error);

        public static Result<T> NotFound<T>(string error)
            => Result<T>.NotFound(error);
        
        public static Result<T> Conflict<T>(string error)
            => Result<T>.Conflict(error);

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
        
        public static Result<T> Unauthorised(string message)
            => new Result<T>()
            {
                Type = ResultType.Unauthorised,
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
        
        public static Result<T> Conflict(string message) => new Result<T>()
        {
            Type = ResultType.Conflict,
            Message = message
        };
    }

    public static class ResultExtensions
    {
        public static Result<T> AsResult<T>(this T value)
            => Result<T>.Success(value);

        public static Result<TReturn> Select<T, TReturn>(this Result<T> result, Func<T, TReturn> select)
            => result.Successful
                ? Result<TReturn>.Success(select(result.Value))
                : Result<TReturn>.FromFailure(result);
    }
}