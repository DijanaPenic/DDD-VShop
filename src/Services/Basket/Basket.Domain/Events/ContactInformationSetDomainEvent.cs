using System;

using VShop.SharedKernel.Infrastructure.Domain;
using VShop.Services.Basket.Domain.Models.CustomerAggregate;

namespace VShop.Services.Basket.Domain.Events
{
    public record ContactInformationSetDomainEvent : IDomainEvent
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public BasketCustomer.GenderType Gender { get; set; }
    }
}