using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private DbContextOptions<RefactorThisContext> _dbContextOption = null!;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            _dbContextOption = new DbContextOptionsBuilder<RefactorThisContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment();
            var failureMessage = "";

            try
            {
                var result = paymentProcessor.ProcessPayment(payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual("There is no invoice matching this payment", failureMessage);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice();

            ctx.Add(invoice);

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("no payment needed", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice
            {
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment>
                {
                    new()
                    {
                        Amount = 10
                    }
                }
            };
            ctx.Add(invoice);
            ctx.SaveChanges();

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new()
                    {
                        Amount = 5
                    }
                }
            };
            ctx.Add(invoice);
            ctx.SaveChanges();

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 6
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            ctx.Add(invoice);
            ctx.SaveChanges();

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment()
            {
                InvoiceReference = invoice.Reference,
                Amount = 6
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice
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
            ctx.Add(invoice);
            ctx.SaveChanges();

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 5
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment> { new() { Amount = 10 } }
            };
            ctx.Add(invoice);
            ctx.SaveChanges();

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 10
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>
                {
                    new()
                    {
                        Amount = 5
                    }
                }
            };
            ctx.Add(invoice);
            ctx.SaveChanges();

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 1
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            using var ctx = new RefactorThisContext(_dbContextOption);
            var invoice = new Invoice
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            ctx.Add(invoice);
            ctx.SaveChanges();

            var paymentProcessor = new InvoicePaymentProcessor(ctx);

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 1
            };

            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }
    }
}