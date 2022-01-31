using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Modules.Contracts;

public interface IModuleClient
{
    Task PublishAsync(object message, CancellationToken cancellationToken = default);
}