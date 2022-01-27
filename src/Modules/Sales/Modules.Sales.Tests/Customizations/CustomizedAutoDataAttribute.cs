using AutoFixture;
using AutoFixture.Xunit2;
using System.Reflection;

using VShop.SharedKernel.Tests;

namespace VShop.Modules.Sales.Tests.Customizations
{
    internal class CustomizedAutoDataAttribute : AutoDataAttribute
    {
        private static readonly IFixture ExtendedFixture;

        static CustomizedAutoDataAttribute()
        {
            IList<ICustomization> customizations = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ICustomization).IsAssignableFrom(t))
                .Select(t => (ICustomization)Activator.CreateInstance(t))
                .ToList();
            
            ExtendedFixture = AppFixture.CommonFixture;

            ExtendedFixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => ExtendedFixture.Behaviors.Remove(b));

            ExtendedFixture.Behaviors.Add(new OmitOnRecursionBehavior());

            foreach (ICustomization customization in customizations)
            {
                ExtendedFixture.Customize(customization);
            }
        }

        public CustomizedAutoDataAttribute() : base (() => ExtendedFixture) { }
    }
}