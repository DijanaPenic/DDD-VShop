using OneOf;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure
{
    public class Result<TData> : ResultBase<Success<TData>>
    {
        private Result(OneOf<Success<TData>, ApplicationError> input) : base(input) { }
        
        public static implicit operator Result<TData>(TData data) => new(new Success<TData>(data));
        public static implicit operator Result<TData>(ApplicationError error) => new(error);
        
        public TData GetData() => AsT0.Value;
    }
    
    public class Result : ResultBase<Success>
    {
        private Result(OneOf<Success, ApplicationError> input) : base(input) { }
        
        public static implicit operator Result(Success success) => new(success);
        public static implicit operator Result(ApplicationError error) => new(error);
        
        public static Success Success => new();
    }
    
    public abstract class ResultBase<TSuccess> : OneOfBase<TSuccess, ApplicationError>, IResult
    {
        protected ResultBase(OneOf<TSuccess, ApplicationError> input) : base(input) { }
        
        public bool IsError(out ApplicationError error)
        {
            error = IsT1 ? AsT1 : default;
            return IsT1;
        }
        
        public static ApplicationError ValidationError(string message)
            => new ValidationError(message);
        public static ApplicationError InternalServerError(string message)
            => new InternalServerError(message);
        public static ApplicationError NotFoundError(string message)
            => new NotFoundError(message);
    }

    public interface IResult : IOneOf
    {

    }
}