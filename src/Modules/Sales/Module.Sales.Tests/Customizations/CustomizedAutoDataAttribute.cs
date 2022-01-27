using AutoFixture;
using AutoFixture.Xunit2;
using System.Reflection;

using VShop.SharedKernel.Tests;

namespace VShop.Modules.Sales.Tests.Customizations
{
    public class CustomizedAutoDataAttribute : AutoDataAttribute
    {
        private static readonly IList<ICustomization> Customizations;

        static CustomizedAutoDataAttribute()
            => Customizations = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ICustomization).IsAssignableFrom(t))
                .Select(t => (ICustomization)Activator.CreateInstance(t))
                .ToList();

        public CustomizedAutoDataAttribute() : base (() =>
        {
            Fixture fixture = AppFixture.CommonFixture;

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            foreach (ICustomization customization in Customizations)
            {
                fixture.Customize(customization);
            }

            return fixture;
        }) { }
    }
}