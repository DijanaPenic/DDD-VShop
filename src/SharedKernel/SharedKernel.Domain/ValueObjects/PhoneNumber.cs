using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.Infrastructure;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }

        [JsonConstructor]
        internal PhoneNumber(string value) => Value = value;

        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.ValidationError("The phone number is required.");
            
            if (string.IsNullOrWhiteSpace(value))
                return Result.ValidationError("The phone number is required.");

            string phoneNumber = value.Trim();

            if (!Regex.IsMatch(phoneNumber, @"^(?:(?:\(?(?:00|\+)([1-4]\d\d|[1-9]\d?)\)?)?[\-\.\ \\\/]?)?((?:\(?\d{1,}\)?[\-\.\ \\\/]?){0,})(?:[\-\.\ \\\/]?(?:#|ext\.?|extension|x)[\-\.\ \\\/]?(\d+))?$"))
                return Result.ValidationError($"{phoneNumber} is not a valid phone number.");
            
            return new PhoneNumber(phoneNumber);
        }
        
        public static implicit operator string(PhoneNumber self) => self.Value;

        public override string ToString() => Value;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}