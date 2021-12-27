using FluentValidation;

namespace VShop.Modules.Catalog.API.Models
{
    public class CategoryRequest
    {
        public string Name { get; init; }
        public string Description { get; init; }
    }
    
    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}