using OneOf;

namespace VShop.SharedKernel.Infrastructure.Errors
{
    [GenerateOneOf]
    public partial class ApplicationError : OneOfBase
        <ValidationError,
        InternalServerError,
        NotFoundError>
    {
        
    }
}