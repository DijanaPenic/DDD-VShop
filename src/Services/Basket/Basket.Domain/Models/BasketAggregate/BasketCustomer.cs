using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.Models.CustomerAggregate
{
    public class BasketCustomer : AggregateRoot
    {
        // TODO - add needed value objects
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MiddleName { get; private set; }
        public string Email { get; private set; }
        public GenderType Gender { get; private set; }
        
        
        public static BasketCustomer Create(EntityId customerId, string firstName, string lastName, string middleName, string email, GenderType gender)
        {
            BasketCustomer customer = new();
            
            customer.Apply
            (
                new ContactInformationSetDomainEvent
                {
                    Id = customerId,
                    FirstName = firstName,
                    LastName = lastName,
                    MiddleName = middleName,
                    Email = email,
                    Gender = gender
                }
            );

            return customer;
        }
        
        protected override void When(object @event)
        {
            switch (@event)
            {
                case ContactInformationSetDomainEvent e:
                    FirstName = e.FirstName;
                    LastName = e.LastName;
                    MiddleName = e.MiddleName;
                    Email = e.Email;
                    Gender = e.Gender;
                    break;
            }
        }
        
        public enum GenderType { Female, Male, Other }
    }
}