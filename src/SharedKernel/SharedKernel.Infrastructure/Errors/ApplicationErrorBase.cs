namespace VShop.SharedKernel.Infrastructure.Errors
{
    public abstract class ApplicationErrorBase
    {
        public string Message { get; }
        protected ApplicationErrorBase(string message) => Message = message;
    }
}