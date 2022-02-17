namespace VShop.SharedKernel.Infrastructure.Errors
{
    public class UnauthorizedError : ApplicationErrorBase
    {
        public UnauthorizedError(string message) : base(message){ }
    }
}