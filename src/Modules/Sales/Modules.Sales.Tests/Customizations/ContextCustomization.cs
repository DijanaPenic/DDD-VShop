using AutoFixture;

using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.Modules.Sales.Tests.Customizations
{
    internal class ContextCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<IContext>(composer => composer.FromFactory(() =>
            new Context(SequentialGuid.Create(), SequentialGuid.Create())));
        }
    }
}