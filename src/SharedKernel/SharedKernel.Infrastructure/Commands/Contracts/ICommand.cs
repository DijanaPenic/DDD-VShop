namespace VShop.SharedKernel.Infrastructure.Commands.Contracts
{
    public interface ICommand : IBaseCommand
    {
        
    }
    
    public interface ICommand<TResponse> : IBaseCommand
    {
        
    }
}