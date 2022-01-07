using NodaTime;
using NodaTime.TimeZones;

using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.Infrastructure.Services
{
    public class ClockService : IClockService
    {
        private readonly IClock _clock;

        public DateTimeZone TimeZone { get; }

        public ClockService() : this(SystemClock.Instance) { }

        public ClockService(IClock clock)
        {
            _clock = clock;

            // TODO - get the current users timezone here instead of hard coding it.
            TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Zagreb");
        }

        public Instant Now => _clock.GetCurrentInstant();

        public LocalDateTime LocalNow => ToLocal(Now);

        public Instant ToInstant(LocalDateTime local)
            => local.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

        public LocalDateTime ToLocal(Instant instant)
            => instant.InZone(TimeZone).LocalDateTime;
    }
}