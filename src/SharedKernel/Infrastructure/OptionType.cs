using OneOf;
using OneOf.Types;

namespace VShop.SharedKernel.Infrastructure
{
    public class Option<TResult> : OneOfBase<TResult, None>
        where TResult : class
    {
        private Option(OneOf<TResult, None> value) : base(value) { }

        public static implicit operator Option<TResult>(TResult value) => new((value is null) ? new None() : value);
        public static implicit operator Option<TResult>(None value) => new(value);

        public (bool isResult, TResult result) TryGetResult() =>
            Match
            (
                result => (true, result),
                none => (false, default)
            );
    }
}