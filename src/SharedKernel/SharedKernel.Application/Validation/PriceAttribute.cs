using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.Application.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PriceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is not decimal price) return new ValidationResult("Invalid format.");

            Result<Price> result = Price.Create(price);

            return result.IsError ? new ValidationResult(result.Error.ToString()) : ValidationResult.Success;
        }
    }
}