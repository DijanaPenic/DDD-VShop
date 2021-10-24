using System;

using VShop.SharedKernel.Domain;
using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.Modules.Sales.Domain.Events;

namespace VShop.Modules.Sales.Domain.Models.ShoppingCart
{
    public class ShoppingCartCustomer : Entity<EntityId>
    {
        private bool _isClosedForUpdates;
        
        public EntityId CustomerId { get; private set; }
        public FullName FullName { get; private set; }
        public EmailAddress EmailAddress { get; private set; }
        public GenderType Gender { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Address DeliveryAddress { get; private set; }

        public int Discount { get; private set; }
        
        public ShoppingCartCustomer(Action<IDomainEvent> applier) : base(applier) { }

        public Option<ApplicationError> SetContactInformation
        (
            FullName fullName, 
            EmailAddress emailAddress, 
            PhoneNumber phoneNumber, 
            GenderType gender
        )
        {
            if (_isClosedForUpdates)
                return ValidationError.Create(@"Updating contact information for the shopping cart is not allowed. 
                                                            The shopping cart has been closed for updates.");
            Apply
            (
                new ContactInformationSetDomainEvent
                {
                    ShoppingCartId = Id,
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
                return ValidationError.Create(@"Updating delivery address for the shopping cart is not allowed. 
                                                            The shopping cart has been closed for updates.");
            Apply
            (
                new DeliveryAddressSetDomainEvent
                {
                    ShoppingCartId = Id,
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
                case ShoppingCartCreatedDomainEvent e:
                    CustomerId = new EntityId(e.CustomerId);
                    Discount = e.CustomerDiscount;
                    Id = new EntityId(e.ShoppingCartId);
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
                case ShoppingCartCheckoutRequestedDomainEvent _:
                    _isClosedForUpdates = true;
                    break;
            }
        }
    }
}