using OneOf;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure
{
    public class Result<TData> : OneOfBase<Success<TData>, ApplicationError>, IResult
    {
        private Result(OneOf<Success<TData>, ApplicationError> input) : base(input) { }
        
        public static implicit operator Result<TData>(TData data) => new(new Success<TData>(data));
        public static implicit operator Result<TData>(ApplicationError error) => new(error);

        public bool IsError(out ApplicationError error)
        {
            error = IsT1 ? AsT1 : default;
            return IsT1;
        }

        public TData GetData() => AsT0.Value;
    }
    
    public class Result : OneOfBase<Success, ApplicationError>, IResult
    {
        private Result(OneOf<Success, ApplicationError> input) : base(input) { }
        
        public static implicit operator Result(Success success) => new(success);
        public static implicit operator Result(ApplicationError error) => new(error);

        public bool IsError(out ApplicationError error)
        {
            error = IsT1 ? AsT1 : null;
            return IsT1;
        }
        
        public static Success Success => new();
    }

    public interface IResult : IOneOf
    {

    }
}