using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.Application.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is not string phoneNumber) return new ValidationResult("Invalid format.");

            Result<PhoneNumber> result = PhoneNumber.Create(phoneNumber);

            return result.IsError ? new ValidationResult(result.Error.ToString()) : ValidationResult.Success;
        }
    }
}