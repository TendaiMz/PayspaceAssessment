using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaxCalculator.Domain.Repository
{
    public interface IRepository<T> where T:class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
       // Task<IEnumerable<T>> FindByAsync(string searchString, CancellationToken cancellationToken);
        Task SaveAsync(T entity, CancellationToken cancellationToken);
    }
}
