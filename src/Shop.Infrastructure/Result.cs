using System.Collections.Generic;
using System.Linq;

namespace Shop.Infrastructure
{
    public class Result
    {
        public bool Success { get; }

        public string Message { get; }

        protected Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static Result Fail(string error)
        {
            return new Result(false, error);
        }

        public static Result Fail<TValue>(TValue value, string error)
        {
            return new Result<TValue>(value, false, error);
        }

        public static Result Ok()
        {
            return new Result(true, null);
        }

        public static Result<TValue> Ok<TValue>(TValue value)
        {
            return new Result<TValue>(value, true, null);
        }

        public static Result<TValue> Ok<TValue>(TValue value, string message)
        {
            return new Result<TValue>(value, true, message);
        }

        public static Result<TValue> Fail<TValue>(string error)
        {
            return new Result<TValue>(default(TValue), false, error);
        }
    }
}
