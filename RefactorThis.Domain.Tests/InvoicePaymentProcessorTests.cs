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
        private IInvoicePaymentProcessorService _paymentProcessor = null!;
        private RefactorThisContext _dbContext = null!;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            _dbContextOption = new DbContextOptionsBuilder<RefactorThisContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
             _dbContext = new RefactorThisContext(_dbContextOption);
             _paymentProcessor = new InvoicePaymentProcessorService(_dbContext);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
        {
            var payment = new Payment();
            var ex = Assert.Throws<InvalidOperationException>(() => _paymentProcessor.ProcessPayment(payment));
            Assert.AreEqual("There is no invoice matching this payment", ex?.Message);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
         
            var invoice = new Invoice();

            _dbContext.Add(invoice);
            
            var payment = new Payment
            {
                InvoiceReference = invoice.Reference
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("no payment needed", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
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
            _dbContext.Add(invoice);
            _dbContext.SaveChanges();
            
            var payment = new Payment
            {
                InvoiceReference = invoice.Reference
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
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
            _dbContext.Add(invoice);
            _dbContext.SaveChanges();
            

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 6
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var invoice = new Invoice
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            _dbContext.Add(invoice);
            _dbContext.SaveChanges();

            var payment = new Payment()
            {
                InvoiceReference = invoice.Reference,
                Amount = 6
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
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
            _dbContext.Add(invoice);
            _dbContext.SaveChanges();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 5
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment> { new() { Amount = 10 } }
            };
            _dbContext.Add(invoice);
            _dbContext.SaveChanges();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 10
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
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
            _dbContext.Add(invoice);
            _dbContext.SaveChanges();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 1
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("another partial payment received, still not fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            _dbContext.Add(invoice);
            _dbContext.SaveChanges();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 1
            };

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice is now partially paid", result);
        }
    }
}