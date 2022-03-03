using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests.AutoMoq
{
    public class FixtureFactory
    {
        public static IFixture Create()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());

            return fixture;
        }
    }
}
