using System;
using Microsoft.AspNetCore.Http;

using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public class Context : IContext
{
    public Guid RequestId { get; set; }
    public Guid CorrelationId { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public IIdentityContext Identity { get; }

    public Context(HttpContext context) : this
    (
        context.TryGetRequestId(),
        context.TryGetCorrelationId(),
        new IdentityContext(context.User),
        context.GetUserIpAddress(),
        context.GetUserAgent()
    )
    {
        
    }

    public Context
    (
        Guid? requestId,
        Guid? correlationId,
        IIdentityContext identity = null,
        string ipAddress = null,
        string userAgent = null
    )
    {
        RequestId = requestId ?? SequentialGuid.Create();
        CorrelationId = correlationId ?? SequentialGuid.Create();
        Identity = identity ?? IdentityContext.Empty;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
}