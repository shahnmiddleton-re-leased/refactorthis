using FluentAssertions;
using FluentResults;

namespace RefactorThis.Domain.Tests
{
    public static class ResultAssertions
    {
        public static void ShouldSucceed(this ResultBase result, string message)
        {
            result.IsSuccess.Should().BeTrue();
            result.Successes[0].Message.Should().Be(message);
        }

        public static void ShouldFail(this ResultBase result, string message)
        {
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(message);
        }

    }
}
