using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using RefactorThis.Application.Common.Exceptions;
using RefactorThis.Application.Invoices.Commands;
using System.Threading.Tasks;

namespace RefactorThis.Application.Tests
{
    using static Testing;

    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        [Test]
        public void ShouldRequireMinimumFields()
        {
            var command = new InvoiceAddPaymentCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            var command = new InvoiceAddPaymentCommand() { Reference = "ABC" };
            FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 0 

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 10 };
            var result = await SendAsync(command);
            result.Should().Equals("no payment needed");
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 10,add payments to 10,

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 10 };
            var result = await SendAsync(command);
            result.Should().Equals("invoice was already fully paid");
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 10,add payments to 5,

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 6 };
            var result = await SendAsync(command);
            result.Should().Equals("the payment is greater than the partial amount remaining");

        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 5 

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 6 };
            var result = await SendAsync(command);
            result.Should().Equals("the payment is greater than the invoice amount");
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 10 , payments 5

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 5 };
            var result = await SendAsync(command);
            result.Should().Equals("final partial payment received, invoice is now fully paid");
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 10 , payments 10

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 5 };
            var result = await SendAsync(command);
            result.Should().Equals("invoice was already fully paid");
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 10 , payments 5

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 1 };
            var result = await SendAsync(command);
            result.Should().Equals("another partial payment received, still not fully paid");
        }

        [Test]
        public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            //TODO: Set data for the invoice or use data
            //Amount = 10  

            var command = new InvoiceAddPaymentCommand() { Reference = "1", Amount = 1 };
            var result = await SendAsync(command);
            result.Should().Equals("invoice is now partially paid");
        }
    }
}