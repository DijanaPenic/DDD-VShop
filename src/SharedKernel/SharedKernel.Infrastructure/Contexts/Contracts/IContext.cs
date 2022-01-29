using System;

namespace VShop.SharedKernel.Infrastructure.Contexts.Contracts;

public interface IContext
{
    Guid RequestId { get; set; }
    Guid CorrelationId { get; }
    string IpAddress { get; }
    string UserAgent { get; }
    IIdentityContext Identity { get; }
}