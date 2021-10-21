using OneOf.Types;

namespace VShop.SharedKernel.Infrastructure
{
    public class ApplicationError
    {
        public string Message { get; }
        public static None None => new();

        public ApplicationError(string message)
        {
            Message = message;
        }
    }
}