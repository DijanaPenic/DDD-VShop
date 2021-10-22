using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.SharedKernel.Infrastructure.Domain.Enums;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;
using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketCustomer : Entity<EntityId>
    {
        private bool _isClosedForUpdates;
        
        public EntityId CustomerId { get; private set; }
        public FullName FullName { get; private set; }
        public EmailAddress EmailAddress { get; private set; }
        public GenderType Gender { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Address DeliveryAddress { get; private set; }

        public int Discount { get; private set; }
        
        public BasketCustomer(Action<IDomainEvent> applier) : base(applier) { }

        public Option<ApplicationError> SetContactInformation
        (
            FullName fullName, 
            EmailAddress emailAddress, 
            PhoneNumber phoneNumber, 
            GenderType gender
        )
        {
            if (_isClosedForUpdates)
                return ValidationError.Create(@"Updating contact information for the basket is not allowed. 
                                                            The basket has been closed for updates.");
            Apply
            (
                new ContactInformationSetDomainEvent
                {
                    BasketId = Id,
                    FirstName = fullName.FirstName,
                    MiddleName = fullName.MiddleName,
                    LastName = fullName.LastName,
                    EmailAddress = emailAddress,
                    PhoneNumber = phoneNumber,
                    Gender = gender
                }
            );
            
            return Option<ApplicationError>.None;
        }

        public Option<ApplicationError> SetDeliveryAddress(Address deliveryAddress)
        {
            if(_isClosedForUpdates)
                return ValidationError.Create(@"Updating delivery address for the basket is not allowed. 
                                                            The basket has been closed for updates.");
            Apply
            (
                new DeliveryAddressSetDomainEvent
                {
                    BasketId = Id,
                    City = deliveryAddress.City,
                    CountryCode = deliveryAddress.CountryCode,
                    PostalCode = deliveryAddress.PostalCode,
                    StateProvince = deliveryAddress.StateProvince,
                    StreetAddress = deliveryAddress.StreetAddress,
                }
            );
            
            return Option<ApplicationError>.None;
        }

        protected override void When(IDomainEvent @event)
        {
            switch (@event)
            {
                case BasketCreatedDomainEvent e:
                    CustomerId = new EntityId(e.CustomerId);
                    Discount = e.CustomerDiscount;
                    Id = new EntityId(e.BasketId);
                    break;
                case ContactInformationSetDomainEvent e:
                    FullName = new FullName(e.FirstName, e.MiddleName, e.LastName);
                    EmailAddress = new EmailAddress(e.EmailAddress);
                    PhoneNumber = new PhoneNumber(e.PhoneNumber);
                    Gender = e.Gender;
                    break;
                case DeliveryAddressSetDomainEvent e:
                    DeliveryAddress = new Address
                    (
                        e.City, 
                        e.CountryCode, 
                        e.PostalCode,
                        e.StateProvince, 
                        e.StreetAddress
                    );
                    break;
                case BasketCheckoutRequestedDomainEvent _:
                    _isClosedForUpdates = true;
                    break;
            }
        }
    }
}