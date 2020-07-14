using System.ComponentModel.DataAnnotations.Schema;

namespace TaxCalculator.Domain.Entities
{
    public class ProgessiveIncomeTaxRate : Entity
    {
        [Column("Rate")]
        public decimal Rate { get; set; }


        [Column("LowerLimit")]
        public decimal LowerLimit { get; set; }


        [Column("UpperLimit")]
        public decimal UpperLimit { get; set; }
    }
}
