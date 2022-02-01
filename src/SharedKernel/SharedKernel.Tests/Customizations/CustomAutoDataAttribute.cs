using AutoFixture;
using AutoFixture.Xunit2;
using System.Reflection;

namespace VShop.SharedKernel.Tests.Customizations
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        private static readonly IFixture ExtendedFixture;

        static CustomAutoDataAttribute()
        {
            List<ICustomization> customizations = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ICustomization).IsAssignableFrom(t))
                .Select(t => (ICustomization)Activator.CreateInstance(t))
                .ToList();
            
            customizations.Add(new ContextCustomization());
            
            ExtendedFixture = AppFixture.CommonFixture;

            ExtendedFixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => ExtendedFixture.Behaviors.Remove(b));

            ExtendedFixture.Behaviors.Add(new OmitOnRecursionBehavior());

            foreach (ICustomization customization in customizations)
                ExtendedFixture.Customize(customization);
        }

        public CustomAutoDataAttribute() : base (() => ExtendedFixture) { }
    }
}