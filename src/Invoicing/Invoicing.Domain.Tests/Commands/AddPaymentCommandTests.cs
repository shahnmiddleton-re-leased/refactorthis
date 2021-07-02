using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Invoicing.Domain.Commands;
using NUnit.Framework;

namespace Invoicing.Domain.Tests.Commands
{
    public class AddPaymentCommandTests
    {
        [Theory, DefaultAutoData]
        public void AddPaymentCommand_VerifyBoundariesForAllConstructors(GuardClauseAssertion guard)
        {
            guard.Verify(typeof(AddPaymentCommand).GetConstructors());
        }

        [Theory, DefaultAutoData]
        public void AddPaymentCommand_ShouldReturnInjectedInvoice(
            [Frozen] Invoice invoice,
            AddPaymentCommand sut)
        {
            sut.Invoice.Should().Be(invoice);
        }

        [Theory, DefaultAutoData]
        public void AddPaymentCommand_ShouldReturnInjectedPayment(
            [Frozen] Payment payment,
            AddPaymentCommand sut)
        {
            sut.Payment.Should().Be(payment);
        }
    }
}
