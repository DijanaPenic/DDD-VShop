using Newtonsoft.Json;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class FullName : ValueObject
    {
        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }
        
        [JsonConstructor]
        internal FullName(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public static Result<FullName> Create(string firstName, string middleName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.ValidationError("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName))
                return Result.ValidationError("Last name cannot be empty.");
            
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