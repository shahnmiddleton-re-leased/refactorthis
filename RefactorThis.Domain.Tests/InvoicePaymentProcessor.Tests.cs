using FluentAssertions;
using NUnit.Framework;
using RefactorThis.Domain.Entities.Invoices;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
        {
            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment();

            Action act = () => paymentProcessor.ProcessPayment(null, payment);
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("There is no invoice matching this payment");
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_InvoiceHasNoAmountWithPayments()
        {
            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };

            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment();

            Action act = () => paymentProcessor.ProcessPayment(invoice, payment);
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment();

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("no payment needed", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment();

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var invoice = new Invoice()
            {
                Amount = 5,
                AmountPaid = 0
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment()
            {
                Amount = 6
            };

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment()
            {
                Amount = 5
            };

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>() { new Payment() { Amount = 10 } }
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment()
            {
                Amount = 10
            };

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment()
            {
                Amount = 1
            };

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsEqualToInvoiceAmount()
        {

            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };


            var paymentProcessor = new InvoiceFactory().CreateInvoicePaymentProcessor();

            var payment = new Payment()
            {
                Amount = 10
            };

            var result = paymentProcessor.ProcessPayment(invoice, payment);

            Assert.AreEqual("invoice is now fully paid", result);
        }
    }
}