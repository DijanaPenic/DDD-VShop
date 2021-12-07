using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Serilog;
using Newtonsoft.Json;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Application.Decorators.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class ErrorCommandDecorator<TCommand> : ICommandDecorator<TCommand, Result>
    {
        private static readonly ILogger Logger = Log.ForContext<ErrorCommandDecorator<TCommand>>();
        
        public async Task<Result> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<Result> next
        )
        {
            try
            {
                return await next();
            }
            catch (ValidationException ex)
            {
                return Result.ValidationError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unhandled error has occurred");
                
                return Result.InternalServerError(JsonConvert.SerializeObject(ex));
            }
        }
    }
    
    public class ErrorCommandDecorator<TCommand, TData> : ICommandDecorator<TCommand, Result<TData>>
    {
        private static readonly ILogger Logger = Log.ForContext<ErrorCommandDecorator<TCommand, TData>>();
        
        public async Task<Result<TData>> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TData>> next
        )
        {
            try
            {
                return await next();
            }
            catch (ValidationException ex) // TODO - check if this can be addressed in value object classes
            {
                return Result.ValidationError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unhandled error has occurred");
                
                return Result.InternalServerError(JsonConvert.SerializeObject(ex));
            }
        }
    }
}