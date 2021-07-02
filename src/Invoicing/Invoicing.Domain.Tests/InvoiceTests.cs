using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Invoicing.Domain.Commands;
using Invoicing.Domain.Rules;
using Invoicing.Domain.Tests.Customizations;
using NSubstitute;
using NUnit.Framework;
using System;

namespace Invoicing.Domain.Tests
{
    public class InvoiceTests
    {
        [Theory, DefaultAutoData]
        public void Invoice_VerifyBoundariesForAllConstructors(GuardClauseAssertion guard)
        {
            guard.Verify(typeof(Invoice).GetConstructors());
        }

        [Theory, DefaultAutoData]
        public void AddPayment_PaymentIsNull_ThrowsArgumentNullException(
            Invoice sut)
        {
            Action act = () => sut.AddPayment(null);

            act.Should()
                .Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("payment");
        }

        [Theory, InvoiceAutoData(0, 0)]
        public void AddPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded(
            Payment payment,
            Invoice sut)
        {
            var actual = sut.AddPayment(payment);

            actual.Should().Be("no payment needed");
        }

        [Theory, InvoiceWithPaymentAutoData(10, 10, 10)]
        public void AddPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid(
            Payment payment,
            Invoice sut)
        {
            var actual = sut.AddPayment(payment);

            actual.Should().Be("invoice was already fully paid");
        }

        [Theory, InvoiceWithPaymentAutoData(10, 5, 5)]
        public void AddPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue(
            Payment payment,
            Invoice sut)
        {
            payment.Amount = 6;

            var actual = sut.AddPayment(payment);

            actual.Should().Be("the payment is greater than the partial amount remaining");
        }

        [Theory, InvoiceAutoData(5, 0)]
        public void AddPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount(
            Payment payment,
            Invoice sut)
        {
            payment.Amount = 6;

            var actual = sut.AddPayment(payment);

            actual.Should().Be("the payment is greater than the invoice amount");
        }

        [Theory, InvoiceWithPaymentAutoData(10, 5, 5)]
        public void AddPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue(
            Payment payment,
            Invoice sut)
        {
            payment.Amount = 5;

            var actual = sut.AddPayment(payment);

            actual.Should().Be("final partial payment received, invoice is now fully paid");
        }

        [Theory, InvoiceWithPaymentAutoData(10, 0, 10)]
        public void AddPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount(
            Payment payment,
            Invoice sut)
        {
            payment.Amount = 10;

            var actual = sut.AddPayment(payment);

            actual.Should().Be("invoice was already fully paid");
        }

        [Theory, InvoiceWithPaymentAutoData(10, 5, 5)]
        public void AddPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue(
            Payment payment,
            Invoice sut)
        {
            payment.Amount = 1;

            var actual = sut.AddPayment(payment);

            actual.Should().Be("another partial payment received, still not fully paid");
        }

        [Theory, InvoiceAutoData(10, 0)]
        public void AddPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount(
            Payment payment,
            Invoice sut)
        {
            payment.Amount = 1;

            var actual = sut.AddPayment(payment);

            actual.Should().Be("invoice is now partially paid");
        }

        [Theory, DefaultAutoData]
        public void AddPayment_FailedValidation_ShouldNotAddPayment(
            Payment payment,
            [Frozen] AbstractRuleEngine<AddPaymentCommand> ruleEngine,
            Invoice sut)
        {
            var expected = sut.AmountPaid;
            ruleEngine.Run(Arg.Any<AddPaymentCommand>()).Returns(new RuleResult(false, null));

            sut.AddPayment(payment);

            sut.AmountPaid.Should().Be(expected);
            sut.Payments.Count.Should().Be(0);
        }

        [Theory, DefaultAutoData]
        public void AddPayment_PassedValidation_ShouldAddPayment(
            Payment payment,
            [Frozen] AbstractRuleEngine<AddPaymentCommand> ruleEngine,
            Invoice sut)
        {
            var expected = sut.AmountPaid + payment.Amount;
            ruleEngine.Run(Arg.Any<AddPaymentCommand>()).Returns(new RuleResult(true, null));

            sut.AddPayment(payment);

            sut.AmountPaid.Should().Be(expected);
            sut.Payments.Count.Should().Be(1);
        }

        [Theory, DefaultAutoData]
        public void Save_ShouldSaveOwnStateThroughInvoiceRepository(
            [Frozen] IInvoiceRepository repository,
            Invoice sut)
        {
            sut.Save();

            repository.Received(1).Update(sut);
        }

        public class InvoiceAutoDataAttribute : DefaultAutoDataAttribute
        {
            public InvoiceAutoDataAttribute(int amount, int amountPaid)
                : base(new InvoiceRuleEngineConcreteCustomization(),
                      new InvoiceWithoutPaymentCustomization(amount, amountPaid))
            {
            }
        }

        public class InvoiceWithPaymentAutoDataAttribute : DefaultAutoDataAttribute
        {
            public InvoiceWithPaymentAutoDataAttribute(int amount, int amountPaid, int paymentAmount)
                : base(new InvoiceRuleEngineConcreteCustomization(),
                      new InvoiceWithPaymentCustomization(amount, amountPaid, paymentAmount))
            {
            }
        }
    }
}