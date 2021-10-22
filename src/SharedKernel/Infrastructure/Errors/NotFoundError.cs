namespace VShop.SharedKernel.Infrastructure.Errors
{
    public class NotFoundError : ApplicationErrorBase
    {
        private NotFoundError(string message) : base(message){ }
        public static ApplicationError Create(string message) => new NotFoundError(message);
    }
}