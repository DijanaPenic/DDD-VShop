using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class EmailAddress : ValueObject
    {
        public string Value { get; }

        [JsonConstructor]
        internal EmailAddress(string value) => Value = value;

        public static Result<EmailAddress> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.ValidationError("The email address is required.");

            string email = value.Trim();

            if (email.Length > 256)
                return Result.ValidationError("The email address is too long.");

            if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                return Result.ValidationError("The email address format is not correct.");
            
            return new EmailAddress(email);
        }
        
        public static implicit operator string(EmailAddress self) => self.Value;

        public override string ToString() => Value;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}