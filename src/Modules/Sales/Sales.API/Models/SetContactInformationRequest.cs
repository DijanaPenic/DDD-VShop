using FluentValidation;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Application.Extensions;

namespace VShop.Services.Sales.API.Models
{
    public record SetContactInformationRequest
    {
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public GenderType Gender { get; init; }
    }
    
    public class SetContactInformationRequestValidator : AbstractValidator<SetContactInformationRequest> {
        public SetContactInformationRequestValidator()
        {
            RuleFor(ci => ci.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(ci => ci.MiddleName).MaximumLength(50);
            RuleFor(ci => ci.LastName).NotEmpty().MaximumLength(50);
            RuleFor(ci => ci.EmailAddress).NotEmpty().EmailAddress();
            RuleFor(ci => ci.PhoneNumber).NotEmpty().PhoneNumber();
            RuleFor(ci => ci.Gender).NotEmpty();
        }
    }
}