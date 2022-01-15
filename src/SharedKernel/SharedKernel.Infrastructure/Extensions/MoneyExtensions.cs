using Google.Type;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class MoneyExtensions
    {
        public static Money ToMoney(this decimal value) => new()
        {
            CurrencyCode = "USD",
            DecimalValue = value
        };
    }
}