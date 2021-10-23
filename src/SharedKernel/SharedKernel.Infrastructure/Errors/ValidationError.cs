namespace VShop.SharedKernel.Infrastructure.Errors
{
    public class ValidationError : ApplicationErrorBase
    {
        private ValidationError(string message) : base(message){ }
        public static ApplicationError Create(string message) => new ValidationError(message);
    }
}