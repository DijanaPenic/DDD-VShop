using System;
using System.Threading.Tasks;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure
{
    public class DatabaseProbe<TDbContext, TSample> : IProbe
        where TDbContext : DbContextBase
    {
        private readonly Func<TDbContext, Task<TSample>> _sampling;
        private readonly Action<TSample> _validation;
        private TSample _sample;
        private string _validationError;

        public DatabaseProbe(Func<TDbContext, Task<TSample>> sampling, Action<TSample> validation)
        {
            _sampling = sampling;
            _validation = validation;
        }

        public bool IsSatisfied()
        {
            try
            {
                _validation(_sample);
                return true;
            }
            catch(Exception ex)
            {
                _validationError = ex.Message;
                return false;
            }
        }

        public async Task SampleAsync()
            => _sample = await IntegrationTestsFixture.ExecuteServiceAsync<TDbContext, TSample>
                (dbContext => _sampling(dbContext));

        public string DescribeFailureTo() => _validationError;
    }
}