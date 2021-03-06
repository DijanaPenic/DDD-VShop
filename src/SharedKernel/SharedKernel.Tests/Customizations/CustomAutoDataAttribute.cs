using AutoFixture;
using AutoFixture.Xunit2;

namespace VShop.SharedKernel.Tests.Customizations
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        private static readonly IFixture ExtendedFixture;

        static CustomAutoDataAttribute()
        {
            ExtendedFixture = AppFixture.CommonFixture;

            ExtendedFixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => ExtendedFixture.Behaviors.Remove(b));

            ExtendedFixture.Behaviors.Add(new OmitOnRecursionBehavior());

            IList<ICustomization> customizations  = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ICustomization).IsAssignableFrom(t) && t.Namespace is not null && !t.Namespace.StartsWith("AutoFixture"))
                .Select(t => (ICustomization)Activator.CreateInstance(t))
                .ToList();
            
            foreach (ICustomization customization in customizations)
                ExtendedFixture.Customize(customization);
        }

        public CustomAutoDataAttribute() : base (() => ExtendedFixture) { }
    }
}