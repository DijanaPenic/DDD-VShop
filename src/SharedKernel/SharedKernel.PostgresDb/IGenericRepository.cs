using System.Threading.Tasks;

namespace VShop.SharedKernel.PostgresDb
{
    public interface IGenericRepository
    {
        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteByKeyAsync<TEntity>(params object[] keyValues) where TEntity : class;
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
    }
}