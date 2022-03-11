using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

namespace Invoicing.Domain.Tests
{
    public class DefaultAutoDataAttribute : AutoDataAttribute
    {
        public DefaultAutoDataAttribute()
             : base(() => new Fixture()
                 .Customize(new AutoNSubstituteCustomization
                 {
                     ConfigureMembers = true
                 }))
        {
        }

        public DefaultAutoDataAttribute(params ICustomization[] customizations)
            : base(() =>
                new Fixture()
                    .Customize(new AutoNSubstituteCustomization
                    {
                        ConfigureMembers = true
                    })
                    .Customize(new CompositeCustomization(customizations)))
        {
        }
    }
}