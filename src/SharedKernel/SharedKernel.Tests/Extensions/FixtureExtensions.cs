using AutoFixture;

namespace VShop.SharedKernel.Tests.Extensions
{
    public static class FixtureExtensions
    {
        public static int CreateInt(this IFixture fixture, int min, int max) 
            => fixture.Create<int>() % (max - min + 1) + min;
        
        public static decimal CreateDecimal(this IFixture fixture, decimal min, decimal max) 
            => fixture.Create<decimal>() % (max - min + 1) + min;
    }
}