using System.Threading.Tasks;

namespace VShop.SharedKernel.EventSourcing
{
    public interface ISubscription
    {
        Task ProjectAsync(object @event); 
    }
}