using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.Repositories;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Billing.API.Application.EventHandlers
{
    public class OrderFinalizedIntegrationEventHandler : IEventHandler<OrderFinalizedIntegrationEvent>
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;

        public OrderFinalizedIntegrationEventHandler
        (
            IPaymentService paymentService,
            IPaymentRepository paymentRepository
        )
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
        }

        public async Task Handle(OrderFinalizedIntegrationEvent @event, CancellationToken cancellationToken)
        {
            if (@event.RefundAmount.DecimalValue == 0) return;
            
            bool isRefundSuccess = await _paymentRepository.IsPaymentSuccessAsync
            (
                @event.OrderId,
                PaymentType.Transfer,
                cancellationToken
            );
            if (isRefundSuccess) return;
            
            Result refundResult = await _paymentService.RefundAsync
            (
                @event.OrderId,
                @event.RefundAmount.DecimalValue,
                cancellationToken
            );

            Payment refund = new()
            {
                Id = SequentialGuid.Create(),
                OrderId = @event.OrderId,
                Status = refundResult.IsError ? PaymentStatus.Failed : PaymentStatus.Success,
                Error = refundResult.IsError ? refundResult.Error.ToString() : string.Empty,
                Type = PaymentType.Refund
            };
            await _paymentRepository.SaveAsync(refund, cancellationToken);
        }
    }
}