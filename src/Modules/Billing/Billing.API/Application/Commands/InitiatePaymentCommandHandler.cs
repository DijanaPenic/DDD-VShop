using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.Modules.Billing.API.Application.Commands
{
    public class InitiatePaymentCommandHandler : ICommandHandler<InitiatePaymentCommand>
    {
        private readonly IPaymentService _paymentService;
        
        public InitiatePaymentCommandHandler(IPaymentService paymentService)
            => _paymentService = paymentService;

        public async Task<Result> Handle(InitiatePaymentCommand command, CancellationToken cancellationToken)
        {
            Result transferResult = await _paymentService.TransferAsync
            (
                command.OrderId,
                command.CardTypeId,
                command.CardNumber,
                command.CardSecurityNumber,
                command.CardholderName,
                command.CardExpiration
            );

            if (transferResult.IsError(out ApplicationError transferError)) return transferError;
            
            // TODO - need to save result in the database
            // TODO - implement outbox pattern
            
            return Result.Success;
        }
    }
    
    public record InitiatePaymentCommand : Command
    {
        public Guid OrderId { get; set; }
        public int CardTypeId { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityNumber { get; set; }
        public string CardholderName { get; set; }
        public DateTime CardExpiration { get; set; }
    }
}