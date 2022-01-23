using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;

// Source: https://enterprisecraftsmanship.com/posts/combining-asp-net-core-attributes-with-value-objects/
namespace VShop.SharedKernel.API.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EntityIdAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is not Guid entityId) return new ValidationResult("Invalid format.");

            Result<EntityId> result = EntityId.Create(entityId);

            return result.IsError ? new ValidationResult(result.Error.ToString()) : ValidationResult.Success;
        }
    }
}