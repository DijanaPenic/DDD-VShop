using System;
using FluentValidation;

namespace VShop.Modules.Catalog.API.Models
{
    public record ProductRequest
    {
        public Guid CategoryId { get; init; }
        public string SKU { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public string PictureUri { get; init; }
        public int AvailableStock { get; init; }
        public int MaxStockThreshold { get; init; }
    }
    
    public class ProductCreateRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductCreateRequestValidator()
        {
            RuleFor(p => p.CategoryId).NotEmpty();
            RuleFor(p => p.SKU).NotEmpty();
            RuleFor(p => p.Name).NotEmpty();
            
            RuleFor(p => p.Price)
                .GreaterThan(0)
                .NotEmpty();
            
            RuleFor(p => p.AvailableStock)
                .GreaterThanOrEqualTo(0)
                .NotEmpty();
            
            RuleFor(p => p.MaxStockThreshold)
                .GreaterThan(0)
                .NotEmpty();
        }
    }
}