using System;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Invoicing.Domain.Tests
{
    public class InvoicePaymentProcessorTests
    {
        [Theory, DefaultAutoData]
        public void StorageQueueIO_VerifyBoundariesForAllConstructors(GuardClauseAssertion guard)
        {
            guard.Verify(typeof(InvoicePaymentProcessor).GetConstructors());
        }

        [Theory, DefaultAutoData]
        public void ProcessPayment_PaymentIsNull_ThrowsArgumentNullException(
            InvoicePaymentProcessor sut)
        {
            Action act = () => sut.ProcessPayment(null);

            act.Should()
                .Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("payment");
        }

        [Theory, DefaultAutoData]
        public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference(
            Payment payment,
            [Frozen] IInvoiceRepository repository,
            InvoicePaymentProcessor sut)
        {
            repository.Get(Arg.Any<string>()).ReturnsForAnyArgs((Invoice)null);

            Action act = () => sut.ProcessPayment(payment);

            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("There is no invoice matching this payment");
        }
    }
}