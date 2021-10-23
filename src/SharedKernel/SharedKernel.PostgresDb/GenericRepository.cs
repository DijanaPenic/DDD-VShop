using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VShop.SharedKernel.PostgresDb
{
    public class GenericRepository
    {
        private readonly ApplicationDbContextBase _dbContext;
        
        public GenericRepository(ApplicationDbContextBase dbContext)
        {
            _dbContext = dbContext;
        }
        
        public Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Add(entity);
            
            return Task.CompletedTask;
        }
        
        public async Task DeleteByKeyAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            TEntity entity = await _dbContext.Set<TEntity>().FindAsync(keyValues);

            _dbContext.Set<TEntity>().Remove(entity);
        }
        
        public Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Remove(entity);
            
            return Task.CompletedTask;
        }
        
        public Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Entry(entity).State = EntityState.Modified;

            return Task.CompletedTask;
        }
    }
}