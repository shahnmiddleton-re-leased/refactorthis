using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using RefactorThis.Domain.Tests.AutoMoq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	public class InvoicePaymentProcessorTests
	{
		[Theory, AutoMoqData]
		public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment
		)
		{
            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
                .Returns<Invoice>(null);

            var failureMessage = string.Empty;
            
            try
			{
				var result = paymentProcessor.ProcessPayment( payment );
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual( "There is no invoice matching this payment", failureMessage );
		}

		[Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice
        )
        {
            invoice.Amount = 0;
            invoice.Payments = null;

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("no payment needed", result);
        }

        [Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice)
        {
            payment.Amount = 10;

            invoice.Amount = 10;
            invoice.Payments = new List<Payment>{ payment };

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice)
        {
            payment.Amount = 6;

            invoice.Amount = 10;
            invoice.Payments = new List<Payment> { payment };

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice)
        {
            payment.Amount = 6;

            invoice.Amount = 5;
            invoice.Payments = new List<Payment>();

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice)
        {
            payment.Amount = 5;

            invoice.Amount = 10;
            invoice.Payments = new List<Payment> { payment };

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice)
        {
            payment.Amount = 10;

            invoice.Amount = 10;
            invoice.Payments = new List<Payment>() { payment };

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice)
        {
            payment.Amount = 1;

            invoice.Amount = 10;
            invoice.Payments = new List<Payment> { payment };

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
        }

        [Theory, AutoMoqData]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount(
            [Frozen] Mock<IInvoiceRepository> repo,
            [Frozen] InvoicePaymentProcessor paymentProcessor,
            Payment payment,
            Invoice invoice)
        {
            payment.Amount = 1;

            invoice.Amount = 10;
            invoice.Payments = new List<Payment>();

            repo.Setup(s => s.GetInvoice(It.IsAny<string>()))
               .Returns(invoice);

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }
    }
}