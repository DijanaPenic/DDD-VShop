using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Dispatchers;

public interface IDispatcher
{
    Task<object> ExecuteCommandAsync(object command, CancellationToken cancellationToken = default);
}