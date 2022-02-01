using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing.Contracts;

namespace VShop.SharedKernel.Tests.IntegrationTests.Probing
{
    public class PostgresDatabaseProbe<TDbContext, TSample> : IProbe 
        where TDbContext : DbContextBase 
        where TSample : class
    {
        private readonly IModuleFixture _moduleFixture;
        private readonly Func<TDbContext, Task<TSample>> _sampling;
        private readonly Action<TSample> _validation;
        private TSample _sample;
        private string _validationError;

        public PostgresDatabaseProbe
        (
            IModuleFixture moduleFixture,
            Func<TDbContext, Task<TSample>> sampling,
            Action<TSample> validation
        )
        {
            _moduleFixture = moduleFixture;
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
            => _sample = await _moduleFixture.ExecuteServiceAsync<TDbContext, TSample>
                (dbContext => _sampling(dbContext));

        public string DescribeFailureTo() => _validationError;
    }
}