using System;

namespace VShop.Modules.Billing.API.Models
{
    public record BaseRequest
    {
        public Guid MessageId { get; init; }
        public Guid CorrelationId { get; init; }
    }
}