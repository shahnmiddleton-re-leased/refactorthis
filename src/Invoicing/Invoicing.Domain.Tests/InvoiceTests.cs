using AutoFixture.Idioms;
using FluentAssertions;
using Invoicing.Domain.Tests.Customizations;
using NUnit.Framework;

namespace Invoicing.Domain.Tests
{
    public class InvoiceTests
    {
        [Theory, DefaultAutoData]
        public void StorageQueueIO_VerifyBoundariesForAllConstructors(GuardClauseAssertion guard)
        {
            guard.Verify(typeof(Invoice).GetConstructors());
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

        public class InvoiceAutoDataAttribute : DefaultAutoDataAttribute
        {
            public InvoiceAutoDataAttribute(int amount, int amountPaid)
                : base(new InvoiceWithoutPaymentCustomization(amount, amountPaid))
            {
            }
        }

        public class InvoiceWithPaymentAutoDataAttribute : DefaultAutoDataAttribute
        {
            public InvoiceWithPaymentAutoDataAttribute(int amount, int amountPaid, int paymentAmount)
                : base(new InvoiceWithPaymentCustomization(amount, amountPaid, paymentAmount))
            {
            }
        }
    }
}