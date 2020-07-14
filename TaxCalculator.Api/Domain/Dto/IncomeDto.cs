using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxCalculator.Domain.Value_Object;

namespace TaxCalculator.Domain.Dto
{
    public class IncomeDto
    {
        public decimal AnnualIncome { get; set; }
        public string PostCode { get; set; }

    }
}
