using System;

namespace CQRS3.Library.Helpers
{
    public class Result<T> : Either<Exception, T>
    {
        public static implicit operator Result<T>(Exception ex) => new Result<T>(ex);
        public static implicit operator Result<T>(T value) => new Result<T>(value);

        public Result(T value)
            : base(value)
        {
        }

        public Result(Exception ex)
            : base(ex)
        {
        }

        public T OrElse(T defValue) => RightOrDefault(defValue);

        public Result<T> OnError(Action<Exception> handler)
        {
            DoLeft(handler);
            return this;
        }
    }

    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T value) =>
            new Result<T>(value);

        public static Result<T> ToResult<T>(this Func<T> getValue) =>
            Result.Execute(getValue);

        public static Result<U> SelectMany<T, U>(this Result<T> value, Func<T, Result<U>> k) =>
            value.Match(e => new Result<U>(e), k);

        public static Result<V> SelectMany<T, U, V>(this Result<T> value, Func<T, Result<U>> k, Func<T, U, V> m) =>
            value.SelectMany(t => k(t).SelectMany(u => m(t, u).ToResult()));
    }

    // this simplifies the code using Result<T>
    public static class Result
    {
        public static Result<T> From<T>(T value) => value.ToResult();

        public static Result<T> Execute<T>(Func<T> getValue)
        {
            try
            {
                return new Result<T>(getValue());
            }
            catch (Exception e)
            {
                return new Result<T>(e);
            }
        }
    }
}