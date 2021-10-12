using System;

using VShop.SharedKernel.EventSourcing;
using VShop.Services.Basket.Domain.Events;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.Domain.Models.CustomerAggregate
{
    public class BasketCustomer : Entity<EntityId>
    {
        // TODO - add needed value objects
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MiddleName { get; private set; }
        public string Email { get; private set; }
        public GenderType Gender { get; private set; }
        
        public BasketCustomer(Action<object> applier) : base(applier) { }

        public void SetContactInformation(string firstName, string lastName, string middleName, string email, GenderType gender)
        {
            Apply
            (
                new ContactInformationSetDomainEvent
                {
                    FirstName = firstName,
                    LastName = lastName,
                    MiddleName = middleName,
                    Email = email,
                    Gender = gender
                }
            );
        }
        
        // TODO - need to add 

        protected override void When(object @event)
        {
            switch (@event)
            {
                case BasketCreatedDomainEvent e:
                    Id = new EntityId(e.CustomerId);
                    break;
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