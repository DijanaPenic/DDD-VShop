﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Serilog;
using Newtonsoft.Json;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Application.Decorators
{
    public class ErrorCommandDecorator<TRequest, TResponse> : ICommandDecorator<TRequest, TResponse>
    {
        private static readonly ILogger Logger = Log.ForContext<LoggingCommandDecorator<TRequest, TResponse>>();
        
        public async Task<Result<TResponse>> Handle
        (
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next
        )
        {
            try
            {
                return await next();
            }
            catch (ValidationException ex)
            {
                return ValidationError.Create(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unhandled error has occurred");

                return InternalServerError.Create(JsonConvert.SerializeObject(ex));
            }
        }
    }
}