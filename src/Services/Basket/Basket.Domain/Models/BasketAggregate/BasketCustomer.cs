using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain.Enums;
using VShop.Services.Basket.Domain.Events;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketCustomer : Entity<EntityId>
    {
        // TODO - add needed value objects
        public FullName FullName { get; private set; }
        public EmailAddress EmailAddress { get; private set; }
        public GenderType Gender { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        
        public BasketCustomer(Action<object> applier) : base(applier) { }

        public void SetContactInformation(FullName fullName, EmailAddress emailAddress, PhoneNumber phoneNumber, GenderType gender)
        {
            Apply
            (
                new ContactInformationSetDomainEvent
                {
                    FirstName = fullName.FirstName,
                    MiddleName = fullName.MiddleName,
                    LastName = fullName.LastName,
                    EmailAddress = emailAddress,
                    PhoneNumber = phoneNumber,
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
                    FullName = new FullName(e.FirstName, e.MiddleName, e.LastName);
                    EmailAddress = new EmailAddress(e.EmailAddress);
                    PhoneNumber = new PhoneNumber(e.PhoneNumber);
                    Gender = e.Gender;
                    break;
            }
        }
    }
}