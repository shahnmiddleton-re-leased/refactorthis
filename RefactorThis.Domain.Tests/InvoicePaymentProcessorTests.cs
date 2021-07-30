using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RefactorThis.Domain.InternationalizationService;
using RefactorThis.Domain.InvoicePaymentProcessorService;
using RefactorThis.Persistence;
using SpecLight;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private DbContextOptions<RefactorThisContext> _dbContextOption = null!;
        private IInvoicePaymentProcessorService _paymentProcessor = null!;
        private RefactorThisContext _dbContext = null!;
        private IInternationalizationService _internationalizationService = null!;

        private Payment? _payment;
        private string? _resultProcessPaymentAsync;
        private Invoice? _invoice;
        private Lazy<Exception>? _exception;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            _dbContextOption = new DbContextOptionsBuilder<RefactorThisContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _dbContext = new RefactorThisContext(_dbContextOption);
            _internationalizationService = new InternationalizationService.InternationalizationService();
            _paymentProcessor = new InvoicePaymentProcessorService.InvoicePaymentProcessorService(_dbContext, _internationalizationService);
        }

        [Test]
        public async Task ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .Given(APayment)
                .WhenAsync(WeProcessThePayment)
                .Catch(out _exception)
                .Then(ExceptionMessageShouldBeEquivalentTo, new InvoiceNotFound())
                .ExecuteAsync();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoice)
                .And(APaymentForThatInvoice)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoiceNoPaymentNeeded())
                .ExecuteAsync();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoiceWithAmount_WithPreviousPaymentOfAmount_, 10, 10)
                .And(APaymentForThatInvoice)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoiceAlreadyFullyPaid())
                .ExecuteAsync();
        }


        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoiceWithAmount_WithPreviousPaymentOfAmount_, 10, 5)
                .And(APaymentForThatInvoiceWithValue_, 6)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoicePartialPaymentTooBig())
                .ExecuteAsync();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoiceWithAmount_, 5)
                .And(APaymentForThatInvoiceWithValue_, 6)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoicePaymentTooBig())
                .ExecuteAsync();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoiceWithAmount_WithPreviousPaymentOfAmount_, 10, 5)
                .And(APaymentForThatInvoiceWithValue_, 5)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoiceFinalPaymentPaid())
                .ExecuteAsync();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoiceWithAmount_WithPreviousPaymentOfAmount_, 10, 10)
                .And(APaymentForThatInvoiceWithValue_, 10)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoiceAlreadyFullyPaid())
                .ExecuteAsync();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoiceWithAmount_WithPreviousPaymentOfAmount_, 10, 5)
                .And(APaymentForThatInvoiceWithValue_, 1)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoicePartialPaymentReceived())
                .ExecuteAsync();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            await new Spec(nameof(ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference))
                .GivenAsync(AnInvoiceWithAmount_, 10)
                .And(APaymentForThatInvoiceWithValue_, 1)
                .WhenAsync(WeProcessThePayment)
                .Then(ResultIsEquivalentTo, new InvoicePartiallyPaid())
                .ExecuteAsync();
        }

        private async Task WeProcessThePayment()
        {
            _resultProcessPaymentAsync = await _paymentProcessor.ProcessPaymentAsync(_payment ?? throw new InvalidOperationException());
        }

        private void APayment()
        {
            _payment = new Payment();
        }


        private void APaymentForThatInvoice()
        {
            _payment = new Payment
            {
                InvoiceReference = _invoice?.Reference ?? throw new InvalidOperationException()
            };
        }

        private void APaymentForThatInvoiceWithValue_(int value)
        {
            APaymentForThatInvoice();
            if (_payment == null)
            {
                throw new InvalidOperationException();
            }
            _payment.Amount = value;
        }

        private Task AnInvoiceWithAmount_(int amount)
        {
            return CreateInvoice(new Invoice(), (invoice) =>
            {
                invoice.Amount = amount;
                return invoice;
            });
        }

        private Task AnInvoiceWithAmount_WithPreviousPaymentOfAmount_(int amount, int paymentAmount)
        {
            return CreateInvoice(new Invoice(), (invoice) =>
            {
                invoice.Amount = amount;
                invoice.Payments = new List<Payment>
                {
                    new()
                    {
                        Amount = paymentAmount
                    }
                };
                return invoice;
            });
        }

        private Task AnInvoice()
        {
            return CreateInvoice(new Invoice(), (invoice) => invoice);
        }


        private Task CreateInvoice(Invoice invoice, Func<Invoice, Invoice> update)
        {
            _invoice = update(invoice);
            _dbContext.Add(_invoice);
            return _dbContext.SaveChangesAsync();
        }

        private void ExceptionMessageShouldBeEquivalentTo(TranslationKey key)
        {
            _exception.Should().NotBeNull();
            _exception!.Value.Message.Should().BeEquivalentTo(_internationalizationService.GetTranslationFromKey(key));
        }

        private void ResultIsEquivalentTo(TranslationKey key)
        {
            _resultProcessPaymentAsync.Should().BeEquivalentTo(_internationalizationService.GetTranslationFromKey(key));
        }

    }
}