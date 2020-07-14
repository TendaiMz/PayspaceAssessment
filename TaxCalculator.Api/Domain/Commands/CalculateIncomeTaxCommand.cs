using MediatR;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Domain.Commands
{
    public class CalculateIncomeTaxCommand : IRequest<IncomeTax>
    {
        public decimal AnnualIncome { get; set; }
        public string PostCode { get; set; }

    }
}
