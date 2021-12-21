using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using Newtonsoft.Json;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Application.Decorators.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class ErrorCommandDecorator<TCommand> : ICommandDecorator<TCommand, Result>
    {
        private readonly ILogger _logger;

        public ErrorCommandDecorator(ILogger logger) => _logger = logger;

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
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled error has occurred");
                
                return Result.InternalServerError(JsonConvert.SerializeObject(ex));
            }
        }
    }
    
    public class ErrorCommandDecorator<TCommand, TData> : ICommandDecorator<TCommand, Result<TData>>
    {
        private readonly ILogger _logger;

        public ErrorCommandDecorator(ILogger logger) => _logger = logger;
        
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled error has occurred");
                
                return Result.InternalServerError(JsonConvert.SerializeObject(ex));
            }
        }
    }
}