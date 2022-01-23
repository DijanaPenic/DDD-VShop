using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.API.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DiscountAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is not int discount) return new ValidationResult("Invalid format.");

            Result<Discount> result = Discount.Create(discount);

            return result.IsError ? new ValidationResult(result.Error.ToString()) : ValidationResult.Success;
        }
    }
}