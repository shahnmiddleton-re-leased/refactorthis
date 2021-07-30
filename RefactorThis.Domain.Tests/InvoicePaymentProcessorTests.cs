using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private IInternationalizationService _internationalizationService = null!;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            _dbContextOption = new DbContextOptionsBuilder<RefactorThisContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
             _dbContext = new RefactorThisContext(_dbContextOption);
             _internationalizationService = new InternationalizationService();
             _paymentProcessor = new InvoicePaymentProcessorService(_dbContext, _internationalizationService);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            var payment = new Payment();
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _paymentProcessor.ProcessPaymentAsync(payment));
            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoiceNotFound()), ex?.Message);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            var invoice = new Invoice();

            _dbContext.Add(invoice);
            
            var payment = new Payment
            {
                InvoiceReference = invoice.Reference
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoiceNoPaymentNeeded()), result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                Payments = new List<Payment>
                {
                    new()
                    {
                        Amount = 10
                    }
                }
            };
            _dbContext.Add(invoice);
            await _dbContext.SaveChangesAsync();
            
            var payment = new Payment
            {
                InvoiceReference = invoice.Reference
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoiceAlreadyFullyPaid()), result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                Payments = new List<Payment>
                {
                    new()
                    {
                        Amount = 5
                    }
                }
            };
            _dbContext.Add(invoice);
            await _dbContext.SaveChangesAsync();
            

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 6
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoicePartialPaymentTooBig()), result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var invoice = new Invoice
            {
                Amount = 5,
                Payments = new List<Payment>()
            };
            _dbContext.Add(invoice);
            await _dbContext.SaveChangesAsync();

            var payment = new Payment()
            {
                InvoiceReference = invoice.Reference,
                Amount = 6
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoicePaymentTooBig()), result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            _dbContext.Add(invoice);
            await _dbContext.SaveChangesAsync();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 5
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoiceFinalPaymentPaid()), result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                Payments = new List<Payment> { new() { Amount = 10 } }
            };
            _dbContext.Add(invoice);
            await _dbContext.SaveChangesAsync();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 10
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoiceAlreadyFullyPaid()), result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                Payments = new List<Payment>
                {
                    new()
                    {
                        Amount = 5
                    }
                }
            };
            _dbContext.Add(invoice);
            await _dbContext.SaveChangesAsync();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 1
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoicePartialPaymentReceived()), result);
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var invoice = new Invoice
            {
                Amount = 10,
                Payments = new List<Payment>()
            };
            _dbContext.Add(invoice);
            await _dbContext.SaveChangesAsync();

            var payment = new Payment
            {
                InvoiceReference = invoice.Reference,
                Amount = 1
            };

            var result = await _paymentProcessor.ProcessPaymentAsync(payment);

            Assert.AreEqual(_internationalizationService.GetTranslationFromKey(new InvoicePartiallyPaid()), result);
        }
    }
}