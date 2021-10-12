using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain.Enums;
using VShop.Services.Basket.Domain.Events;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketCustomer : Entity<EntityId>
    {
        public FullName FullName { get; private set; }
        public EmailAddress EmailAddress { get; private set; }
        public GenderType Gender { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Address DeliveryAddress { get; set; }
        
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

        public void SetDeliveryAddress(Address deliveryAddress)
        {
            Apply
            (
                new DeliveryAddressSetDomainEvent
                {
                    City = deliveryAddress.City,
                    CountryCode = deliveryAddress.CountryCode,
                    PostalCode = deliveryAddress.PostalCode,
                    StateProvince = deliveryAddress.StateProvince,
                    StreetAddress = deliveryAddress.StreetAddress,
                }
            );
        }

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
                case DeliveryAddressSetDomainEvent e:
                    DeliveryAddress = new Address(e.City, e.CountryCode, e.PostalCode, e.StateProvince, e.StreetAddress);
                    break;
            }
        }
    }
}