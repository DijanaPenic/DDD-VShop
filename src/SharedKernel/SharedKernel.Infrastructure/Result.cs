using OneOf;
using OneOf.Types;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Serialization;

namespace VShop.SharedKernel.Infrastructure
{
    public class Result<TData> : ResultBase<Success<TData>>
    {
        private Result(OneOf<Success<TData>, ApplicationError> input) : base(input) { }
        public static implicit operator Result<TData>(TData data) => new(new Success<TData>(data));
        public static implicit operator Result<TData>(ApplicationError error) => new(error);
        public TData Data => AsT0.Value;
        public override string ToString() => IsError 
            ? Error.ToString() 
            : JsonConvert.SerializeObject(Data, DefaultJsonSerializer.Settings);
    }
    
    public class Result : ResultBase<Success>
    {
        private Result(OneOf<Success, ApplicationError> input) : base(input) { }
        public static implicit operator Result(Success success) => new(success);
        public static implicit operator Result(ApplicationError error) => new(error);
        public static Success Success => new();
        public override string ToString() => IsError ? Error.ToString() : "Success";
    }
    
    public abstract class ResultBase<TSuccess> : OneOfBase<TSuccess, ApplicationError>, IResult
    {
        protected ResultBase(OneOf<TSuccess, ApplicationError> input) : base(input) { }
        public bool IsError => IsT1;
        public ApplicationError Error => AsT1;
        public static ApplicationError ValidationError(string message) 
            => new ValidationError(message);
        public static ApplicationError ValidationError(IEnumerable<IdentityError> errors) 
            => new ValidationError(string.Join(",", errors.Select(e => e.Description).ToArray()));
        public static ApplicationError InternalServerError(string message) 
            => new InternalServerError(message);
        public static ApplicationError NotFoundError(string message) 
            => new NotFoundError(message);
        public static ApplicationError Unauthorized(string message) 
            => new NotFoundError(message);
    }

    public interface IResult : IOneOf { }
}