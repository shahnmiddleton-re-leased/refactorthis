using NUnit.Framework;

namespace RefactorThis.Domain.Tests.TestHelpers
{
    [TestFixture]
    public abstract class SystemUnderTestIs<T> where T : class
    {
        private T _sut;

        // ReSharper disable once InconsistentNaming
        public T SUT => _sut ?? (_sut = CreateSut());

        protected abstract void Initialize();
        protected abstract T CreateSut();

        [SetUp]
        public void SetUp()
        {
            _sut = null;
            Initialize();
        }
    }
}