using System.Threading.Tasks;

namespace VShop.SharedKernel.EventSourcing
{
    public interface ISubscription
    {
        Task Project(object @event); 
    }
}