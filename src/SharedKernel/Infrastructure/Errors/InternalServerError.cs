namespace VShop.SharedKernel.Infrastructure.Errors
{
    public class InternalServerError : ApplicationErrorBase
    {
        private InternalServerError(string message) : base(message){ }
        public static ApplicationError Create(string message) => new InternalServerError(message);
    }
}