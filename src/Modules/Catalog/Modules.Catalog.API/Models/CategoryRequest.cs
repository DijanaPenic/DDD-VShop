using FluentValidation;

namespace VShop.Modules.Catalog.API.Models
{
    internal class CategoryRequest
    {
        public string Name { get; init; }
        public string Description { get; init; }
    }
    
    internal class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}