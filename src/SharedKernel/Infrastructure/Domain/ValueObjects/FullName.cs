using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

[assembly: InternalsVisibleTo("VShop.Services.Basket.Domain")]
namespace VShop.SharedKernel.Infrastructure.Domain.ValueObjects
{
    public class FullName : ValueObject
    {
        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }
        
        internal FullName(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public static FullName Create(string firstName, string middleName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ValidationException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ValidationException("Last name cannot be empty.");
            
            return new FullName(firstName, middleName, lastName);
        }

        public override string ToString() => $"{FirstName} {MiddleName} {LastName}";
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return MiddleName;
            yield return LastName;
        }
    }
}