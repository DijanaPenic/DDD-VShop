namespace VShop.SharedKernel.Infrastructure.Errors
{
    public class NotFoundError : ApplicationErrorBase
    {
        public NotFoundError(string message) : base(message){ }
    }
}