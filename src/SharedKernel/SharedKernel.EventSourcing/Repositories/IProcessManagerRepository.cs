using System;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.SharedKernel.EventSourcing.Repositories
{
    public interface IProcessManagerRepository<TProcess>
        where TProcess : ProcessManager
    {
        Task SaveAsync(TProcess processManager);
        
        Task<bool> ExistsAsync(Guid processManagerId);
        
        Task<TProcess> LoadAsync(Guid processManagerId);
    }
}