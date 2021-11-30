namespace VShop.SharedKernel.Infrastructure.Errors
{
    public class ValidationError : ApplicationErrorBase
    {
        public ValidationError(string message) : base(message){ }
    }
}