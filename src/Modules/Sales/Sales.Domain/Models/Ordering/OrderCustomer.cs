using System;

using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Models.Ordering
{
    public class OrderCustomer : Entity<EntityId>
    {
        public EntityId CustomerId { get; private set; }
        public FullName FullName { get; private set; }
        public EmailAddress EmailAddress { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Address DeliveryAddress { get; private set; }
        public Discount Discount { get; private set; }
        
        public OrderCustomer(Action<IDomainEvent> applier) : base(applier) { }
        
        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch (@event)
            {
                case OrderPlacedDomainEvent e:
                    Id = new EntityId(e.OrderId);
                    CustomerId = new EntityId(e.CustomerId);
                    Discount = new Discount(e.CustomerDiscount);
                    FullName = new FullName(e.FirstName, e.MiddleName, e.LastName);
                    EmailAddress = new EmailAddress(e.EmailAddress);
                    PhoneNumber = new PhoneNumber(e.PhoneNumber);
                    DeliveryAddress = new Address
                    (
                        e.City, 
                        e.CountryCode, 
                        e.PostalCode,
                        e.StateProvince, 
                        e.StreetAddress
                    );
                    break;
            }
        }
    }
}