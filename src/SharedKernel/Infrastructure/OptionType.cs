using OneOf;
using OneOf.Types;

namespace VShop.SharedKernel.Infrastructure
{
    public class Option<TResult> : OneOfBase<TResult, None>
        where TResult : class
    {
        private Option(OneOf<TResult, None> value) : base(value) { }

        public static implicit operator Option<TResult>(TResult value) => new((value is null) ? None : value);
        public static implicit operator Option<TResult>(None value) => new(value);

        private (bool isSome, TResult result) TryGetResult() =>
          Match
          (
              some => (true, some),
              none => (false, default)
          );
        
        public bool IsSome(out TResult value)
        {
            (bool isSome, TResult result) = TryGetResult();
            
            value = result;
            return isSome;
        }
        
        public static None None => new();
    }
}