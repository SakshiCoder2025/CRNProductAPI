using FluentValidation;
using CRNProductAPI.Models.DTOs;

namespace CRNProductAPI.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(255).WithMessage("Product name cannot exceed 255 characters");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required")
                .MaximumLength(100).WithMessage("CreatedBy cannot exceed 100 characters");
        }
    }
}