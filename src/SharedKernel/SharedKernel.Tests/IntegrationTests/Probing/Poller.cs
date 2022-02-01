using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing.Contracts;

namespace VShop.SharedKernel.Tests.IntegrationTests.Probing
{
    public class Poller
    {
        private readonly IClockService _clockService;
        private readonly int _timeoutMillis;
        private readonly int _pollDelayMillis;

        public Poller(IClockService clockService, int timeoutMillis)
        {
            _clockService = clockService;
            _timeoutMillis = timeoutMillis;
            _pollDelayMillis = 1000;
        }

        public async Task CheckAsync(IProbe probe)
        {
            Timeout timeout = new (_clockService, _timeoutMillis);
            
            while (!probe.IsSatisfied())
            {
                if (timeout.HasTimedOut())
                {
                    throw new AssertErrorException(DescribeFailureOf(probe));
                }

                await Task.Delay(_pollDelayMillis);
                await probe.SampleAsync();
            }
        }

        public async Task<T> GetAsync<T>(IProbe<T> probe)
            where T : class
        {
            Timeout timeout = new (_clockService, _timeoutMillis);
            T sample = null;
            
            while (!probe.IsSatisfied(sample))
            {
                if (timeout.HasTimedOut())
                {
                    throw new AssertErrorException(DescribeFailureOf(probe));
                }

                await Task.Delay(_pollDelayMillis);
                sample = await probe.GetSampleAsync();
            }

            return sample;
        }

        private static string DescribeFailureOf(IProbe probe) => probe.DescribeFailureTo();

        private static string DescribeFailureOf<T>(IProbe<T> probe) => probe.DescribeFailureTo();
    }
}