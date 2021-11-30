namespace VShop.SharedKernel.Infrastructure.Errors
{
    public class InternalServerError : ApplicationErrorBase
    {
        public  InternalServerError(string message) : base(message){ }
    }
}