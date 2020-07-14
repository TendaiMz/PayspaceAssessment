using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxCalculator.Domain.Commands;

namespace TaxCalculator.Domain.Validation
{
    public class CalculateTaxCommandValidator: AbstractValidator<CalculateIncomeTaxCommand>
    {
        public CalculateTaxCommandValidator()
        {
            RuleFor(x => x.AnnualIncome).GreaterThan(0).WithMessage("Annual income should be greater than 0");
            RuleFor(x => x.PostCode).NotEmpty().WithMessage("Post code cannot be empty");
        }
    }
}
