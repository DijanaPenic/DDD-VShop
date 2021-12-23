using System;
using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Application.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    public record BaseRequest
    {
        [Required, EntityId]
        public Guid MessageId { get; init; }
        
        [Required, EntityId]
        public Guid CorrelationId { get; init; }
    }
}