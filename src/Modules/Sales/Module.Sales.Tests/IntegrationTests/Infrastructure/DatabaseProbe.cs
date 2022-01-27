using VShop.SharedKernel.Tests.IntegrationTests.Probing;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal class DatabaseProbe<TDatabase, TSample> : IProbe
    {
        private readonly Func<TDatabase, Task<TSample>> _sampling;
        private readonly Action<TSample> _validation;
        private TSample _sample;
        private string _validationError;

        public DatabaseProbe(Func<TDatabase, Task<TSample>> sampling, Action<TSample> validation)
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
            => _sample = await IntegrationTestsFixture.ExecuteServiceAsync<TDatabase, TSample>
                (
                    dbContext => _sampling(dbContext)
                );

        public string DescribeFailureTo() => _validationError;
    }
}