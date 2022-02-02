using AutoFixture;
using AutoFixture.Xunit2;

namespace VShop.SharedKernel.Tests.Customizations
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        private static readonly IFixture ExtendedFixture;

        static CustomAutoDataAttribute()
        {
            IList<ICustomization> customizations  = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ICustomization).IsAssignableFrom(t) && t.Namespace != null && !t.Namespace.StartsWith("AutoFixture"))
                .Select(t => (ICustomization)Activator.CreateInstance(t))
                .ToList();
            
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