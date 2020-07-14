using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxCalculator.Utils;

namespace TaxCalculator.Domain.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IOptions<ConnectionSettings> _dbSettings;      
        public Repository(IOptions<ConnectionSettings> dbSettings)
        {
            _dbSettings = dbSettings;
           
            DapperTypeMapper.Init();
        }


        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

        
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            var tableName = typeof(T).Name;
                       
            using (var connection = GetConnection())
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QueryAsync<T>($"SELECT * FROM {tableName}");
            };

        }


        public async Task SaveAsync(T entity, CancellationToken cancellationToken)
        {
            var insertQuery = GenerateInsertQuery();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync(insertQuery, entity);
            }
        }
        private string GenerateInsertQuery()
        {
            var tableName = typeof(T).Name;
            var insertQuery = new StringBuilder($"INSERT INTO {tableName} ");

            insertQuery.Append("(");
            var properties = GenerateCollectionOfProperties(GetProperties);
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");
            return insertQuery.ToString();
        }

        private static List<string> GenerateCollectionOfProperties(IEnumerable<PropertyInfo> collectionOfProperties)
        {
            return (from prop in collectionOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }

        SqlConnection GetConnection() => new SqlConnection(_dbSettings.Value.TaxCalculatorDatabase);


    }
}

