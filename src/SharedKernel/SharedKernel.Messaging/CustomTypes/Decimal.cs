namespace VShop.SharedKernel.Messaging.CustomTypes
{
    public partial class Decimal
    {
        private const decimal NanoFactor = 1_000_000_000;

        public Decimal(long units, int nanos)
        {
            Units = units;
            Nanos = nanos;
        }

        public static implicit operator decimal(Decimal value) => value.Units + value.Nanos / NanoFactor;

        public static implicit operator Decimal(decimal value)
        {
            long units = decimal.ToInt64(value);
            int nanos = decimal.ToInt32((value - units) * NanoFactor);
            
            return new Decimal(units, nanos);
        }
    }
}