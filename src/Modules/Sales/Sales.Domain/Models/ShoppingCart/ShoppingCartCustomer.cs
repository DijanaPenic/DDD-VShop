using System;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.Modules.Sales.Domain.Events;

namespace VShop.Modules.Sales.Domain.Models.ShoppingCart
{
    public class ShoppingCartCustomer : Entity<EntityId>
    {
        private bool _isClosedForUpdates;
        
        public EntityId CustomerId { get; private set; }
        public FullName FullName { get; private set; }
        public EmailAddress EmailAddress { get; private set; }
        public Gender Gender { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Address DeliveryAddress { get; private set; }
        public Discount Discount { get; private set; }
        
        public ShoppingCartCustomer(Action<IDomainEvent> applier) : base(applier) { }

        public Result SetContactInformation
        (
            FullName fullName, 
            EmailAddress emailAddress, 
            PhoneNumber phoneNumber, 
            Gender gender
        )
        {
            if (_isClosedForUpdates)
                return Result.ValidationError(@"Updating contact information for the shopping cart is not allowed. 
                                                          The shopping cart has been closed for updates.");
            RaiseEvent
            (
                new ShoppingCartContactInformationSetDomainEvent
                (
                    Id,
                    fullName.FirstName,
                    fullName.MiddleName,
                    fullName.LastName,
                    emailAddress,
                    phoneNumber,
                    gender
                )
            );
            
            return Result.Success;
        }

        public Result SetDeliveryAddress(Address deliveryAddress)
        {
            if(_isClosedForUpdates)
                return Result.ValidationError(@"Updating delivery address for the shopping cart is not allowed. 
                                                            The shopping cart has been closed for updates.");
            RaiseEvent
            (
                new ShoppingCartDeliveryAddressSetDomainEvent
                (
                    Id,
                    deliveryAddress.City,
                    deliveryAddress.CountryCode,
                    deliveryAddress.PostalCode,
                    deliveryAddress.StateProvince,
                    deliveryAddress.StreetAddress
                )
            );
            
            return Result.Success;
        }

        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch (@event)
            {
                case ShoppingCartCreatedDomainEvent e:
                    CustomerId = new EntityId(e.CustomerId);
                    Discount = new Discount(e.CustomerDiscount);
                    Id = new EntityId(e.ShoppingCartId);
                    break;
                case ShoppingCartContactInformationSetDomainEvent e:
                    FullName = new FullName(e.FirstName, e.MiddleName, e.LastName);
                    EmailAddress = new EmailAddress(e.EmailAddress);
                    PhoneNumber = new PhoneNumber(e.PhoneNumber);
                    Gender = e.Gender;
                    break;
                case ShoppingCartDeliveryAddressSetDomainEvent e:
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