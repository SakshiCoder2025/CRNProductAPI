using FluentValidation;
using CRNProductAPI.Models.DTOs;

namespace CRNProductAPI.Validators
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(255).WithMessage("Product name cannot exceed 255 characters");

            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithMessage("ModifiedBy is required")
                .MaximumLength(100).WithMessage("ModifiedBy cannot exceed 100 characters");
        }
    }
}