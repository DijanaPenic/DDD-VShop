using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.Application.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ProductQuantityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is not int productQuantity) return new ValidationResult("Invalid format.");

            Result<ProductQuantity> result = ProductQuantity.Create(productQuantity);

            return result.IsError ? new ValidationResult(result.Error.ToString()) : ValidationResult.Success;
        }
    }
}