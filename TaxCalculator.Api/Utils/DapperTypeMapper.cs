using Dapper;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TaxCalculator.Domain.Entities;

namespace TaxCalculator.Utils
{
    internal static class DapperTypeMapper
    {

        internal static void Init()
        {
            SqlMapper.SetTypeMap(typeof(IncomeTax), Map<IncomeTax>());
            SqlMapper.SetTypeMap(typeof(ProgessiveIncomeTaxRate), Map<ProgessiveIncomeTaxRate>());
            SqlMapper.SetTypeMap(typeof(TaxCalculationType), Map<TaxCalculationType>());
        }

        private static SqlMapper.ITypeMap Map<T>()
        {
            var modelType = typeof(T);

            return new CustomPropertyTypeMap(modelType,
                (type, columnName) =>
                {
                    return type
                        .GetProperties()
                        .FirstOrDefault(prop => prop
                            .GetCustomAttributes(false)
                            .OfType<ColumnAttribute>()
                            .Any(attr => attr.Name.Split(',', ';', '|').Contains(columnName)));
                });
        }
    }
}

