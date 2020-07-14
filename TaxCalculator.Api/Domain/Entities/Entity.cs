using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxCalculator.Domain.Entities
{
    public class Entity
    {
        [Column("ID")]
        [Description("ignore")]
        public int ID { get; set; }
    }
}
