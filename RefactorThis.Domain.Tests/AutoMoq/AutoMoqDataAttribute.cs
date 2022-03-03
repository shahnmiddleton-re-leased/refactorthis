using AutoFixture.NUnit3;

namespace RefactorThis.Domain.Tests.AutoMoq
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(FixtureFactory.Create) { }
    }
}
