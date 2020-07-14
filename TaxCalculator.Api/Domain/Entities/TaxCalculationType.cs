using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TaxCalculator.Domain.Value_Object;

namespace TaxCalculator.Domain.Entities
{
    public class TaxCalculationType:Entity
    {
        [Column("Name")]
        public string Name { get; set; }
        [Column("PostCode")]
        public string PostCode { get; set; }
    }
}
