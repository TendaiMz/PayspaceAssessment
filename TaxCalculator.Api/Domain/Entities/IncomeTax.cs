using System.ComponentModel.DataAnnotations.Schema;

namespace TaxCalculator.Domain.Entities
{
    public class IncomeTax : Entity
    {
        [Column("IncomeAmount")]
        public decimal IncomeAmount { get; set; }
        [Column("PostCode")]
        public string PostCode { get; set; }
        [Column("Tax")]
        public decimal Tax { get; set; }
    }
}
