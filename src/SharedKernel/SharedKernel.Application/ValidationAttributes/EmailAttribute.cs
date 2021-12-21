using System;
using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.Application.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is not string email) return new ValidationResult("Invalid format.");

            Result<EmailAddress> result = EmailAddress.Create(email);

            return result.IsError ? new ValidationResult(result.Error.ToString()) : ValidationResult.Success;
        }
    }
}