using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Application.Models;
using RefactorThis.Application.Services;
using RefactorThis.Domain.Entities.Invoice;
using RefactorThis.Persistence;

namespace RefactorThis.Application.Tests.Services
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {

        [Test]
        public void Instantiate_Should_ThrowException_When_RepoIsNull()
        {
            void Action() => new InvoicePaymentProcessor(null);

            Assert.Throws<ArgumentNullException>(Action);
        }

        [Test]
        public void Instantiate_Should_ThrowException_When_PaymentRefIsEmpty()
        {
            var repo = new InvoiceRepository();

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 10,
                Reference = ""
            };

            void Action() => paymentProcessor.ProcessPayment(payment);

            var ex = Assert.Throws<ArgumentException>(Action);
            Assert.That(ex.Message, Is.EqualTo("Payment must include a reference."));
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            var repo = new InvoiceRepository();

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 10,
                Reference = "ref"
            };
            var failureMessage = "";

            try
            {
                var result = paymentProcessor.ProcessPayment(payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual("There is no invoice matching this payment.", failureMessage);
        }

        //[Test]
        //public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        //{
        //    var repo = new InvoiceRepository();

        //    var invoice = new Invoice(repo)
        //    {
        //        Amount = 0,
        //        AmountPaid = 0,
        //        Payments = null
        //    };

        //    repo.Add(invoice);

        //    var paymentProcessor = new InvoicePaymentProcessor(repo);

        //    var payment = new Payment();

        //    var result = paymentProcessor.ProcessPayment(payment);

        //    Assert.AreEqual("no payment needed", result);
        //}

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice(10, 10,
                new List<Payment>
                {
                    new Payment(10, "ref")
                }
            );
            repo.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 10,
                Reference = "ref"
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(10, 5,
                new List<Payment>
                {
                    new Payment(5, "ref")
                }
            );
            repo.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 6,
                Reference = "ref"
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(5, 0,
                new List<Payment>()
            );
            repo.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 6,
                Reference = "ref"
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(10, 5,
                new List<Payment>
                {
                    new Payment(5, "ref")
                }
            );
            repo.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 5,
                Reference = "ref"
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(10, 10,
                new List<Payment>()
                {
                    new Payment(10, "ref")
                }
            );
            repo.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 10,
                Reference = "ref"
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(10, 5,
                new List<Payment>
                {
                    new Payment(5, "ref")
                }
            );
            repo.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 1,
                Reference = "ref"
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var repo = new InvoiceRepository();
            var invoice = new Invoice(10, 0,
                new List<Payment>()
            );
            repo.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            var payment = new PaymentDto
            {
                Amount = 1,
                Reference = "ref"
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }
    }
}
