using NodaTime;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.Tests.IntegrationTests.Probing
{
    public class Timeout
    {
        private readonly Instant _endTime;
        private readonly IClockService _clockService;

        public Timeout(IClockService clockService, int durationMillis)
        {
            _clockService = clockService;
            _endTime = _clockService.Now.Plus(Duration.FromMilliseconds(durationMillis));
        }

        public bool HasTimedOut() => _clockService.Now > _endTime;
    }
}