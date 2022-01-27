using FluentValidation;

namespace VShop.SharedKernel.Application.Extensions
{
    public static class FluentValidationExtensions
    {
        public static void Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            ruleBuilder
                .Length(8, 100)
                .Matches("[A-Z]").WithMessage("{PropertyName} must contain a upper case ASCII character.")
                .Matches("[a-z]").WithMessage("{PropertyName} must contain a lower case ASCII character.")
                .Matches("[0-9]").WithMessage("{PropertyName} must contain a digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("{PropertyName} must contain a non-alphanumeric character.");
        }

        public static void PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            ruleBuilder
                .Matches(@"^(?:(?:\(?(?:00|\+)([1-4]\d\d|[1-9]\d?)\)?)?[\-\.\ \\\/]?)?((?:\(?\d{1,}\)?[\-\.\ \\\/]?){0,})(?:[\-\.\ \\\/]?(?:#|ext\.?|extension|x)[\-\.\ \\\/]?(\d+))?$")
                .WithMessage("{PropertyName} is not a valid phone number.");
        }
    }
}