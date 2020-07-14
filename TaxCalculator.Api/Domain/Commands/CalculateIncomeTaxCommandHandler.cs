using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaxCalculator.Domain.Entities;
using TaxCalculator.Domain.Repository;
using TaxCalculator.Utils;

namespace TaxCalculator.Domain.Commands
{
    public class CalculateIncomeTaxCommandHandler : IRequestHandler<CalculateIncomeTaxCommand, IncomeTax>
    {
        private readonly IRepository<TaxCalculationType> _taxCalculationTypeRepo;
        private readonly IRepository<IncomeTax> _incomeTaxRepo;
        private readonly IRepository<ProgessiveIncomeTaxRate> _progressiveTaxRepo;

        public CalculateIncomeTaxCommandHandler(
            IRepository<TaxCalculationType> taxCalculationTypeRepo,
            IRepository<ProgessiveIncomeTaxRate> progressiveTaxRepo,
            IRepository<IncomeTax> incomeTaxRepo)
        {
            _taxCalculationTypeRepo = taxCalculationTypeRepo ?? throw new ArgumentNullException($"{nameof(taxCalculationTypeRepo)} cannot be null");
            _progressiveTaxRepo = progressiveTaxRepo ?? throw new ArgumentNullException($"{nameof(progressiveTaxRepo)} cannot be null");
            _incomeTaxRepo = incomeTaxRepo ?? throw new ArgumentNullException($"{nameof(incomeTaxRepo)} cannot be null");

        }



        public async Task<IncomeTax> Handle(CalculateIncomeTaxCommand request, CancellationToken cancellationToken)
        {

            var taxCalculationTypes = await _taxCalculationTypeRepo.GetAllAsync(cancellationToken);

            if (taxCalculationTypes is null)
            {
                throw new ArgumentNullException($"{nameof(taxCalculationTypes)} cannot be null");
            }

            var taxCalculationType = taxCalculationTypes.First(x => x.PostCode == request.PostCode) ?? throw new ArgumentNullException($" TaxCalculationType cannot be null"); ;


            IncomeTax incomeTax = new IncomeTax { IncomeAmount = request.AnnualIncome, PostCode = request.PostCode };

            if (taxCalculationType.Name == Constants.FlatValue)
            {
                incomeTax.Tax = GetFlatValueTaxAmount(request.AnnualIncome);
            }

            if (taxCalculationType.Name == Constants.FlatRate)
            {
                incomeTax.Tax = GetFlatRateTaxAmount(request.AnnualIncome);
            }

            if (taxCalculationType.Name == Constants.Progressive)
            {
                incomeTax.Tax = await GetProgressiveTaxAmount(request.AnnualIncome, cancellationToken);
            }


            await _incomeTaxRepo.SaveAsync(incomeTax, cancellationToken);
            var incomeTaxes = await _incomeTaxRepo.GetAllAsync(cancellationToken);
            return incomeTaxes.LastOrDefault();

        }

        decimal GetFlatValueTaxAmount(decimal income)
        {
            if (income < 200000)
            {
                return (decimal).05 * income;
            }
            return 10000;
        }

        decimal GetFlatRateTaxAmount(decimal income)
        {

            return (decimal).175 * income;

        }

        async Task<decimal> GetProgressiveTaxAmount(decimal income, CancellationToken cancellationToken)
        {
            decimal calculatedIncomeTax = 0;
            var progressiveTaxRates = await _progressiveTaxRepo.GetAllAsync(cancellationToken);
            if (progressiveTaxRates is null)
            {
                throw new ArgumentNullException($"{nameof(progressiveTaxRates)} cannot be null");
            }
            foreach (var progessiveIncomeTaxRate in progressiveTaxRates)
            {

                if (progessiveIncomeTaxRate.UpperLimit >= income)
                {
                    return (progessiveIncomeTaxRate.Rate/100) * income;

                }

                if (progessiveIncomeTaxRate.LowerLimit < income && income >= progessiveIncomeTaxRate.UpperLimit)
                {
                    return (progessiveIncomeTaxRate.Rate / 100) * income;
                }

                if (progessiveIncomeTaxRate.LowerLimit <= income)
                {
                    return (progessiveIncomeTaxRate.Rate / 100) * income;
                };
            }
            return calculatedIncomeTax;

        }

    }

}


