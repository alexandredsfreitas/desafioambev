using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for SaleItem entity
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");
            
        RuleFor(item => item.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required");
            
        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(20)
            .WithMessage("Cannot sell more than 20 identical items");
            
        RuleFor(item => item.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero");
            
        // Business rule: Purchases below 4 items cannot have a discount
        RuleFor(item => item.DiscountPercentage)
            .Equal(0)
            .When(item => item.Quantity < 4)
            .WithMessage("Purchases below 4 items cannot have a discount");
    }
}