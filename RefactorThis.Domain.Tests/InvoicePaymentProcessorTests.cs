using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Domain.Model;
using RefactorThis.Domain.Services;
using RefactorThis.Domain.Tests.TestHelpers;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessor_Should
    {
        private Payment _payment;
        private Invoice _invoice;

        [Test]
        public void ReturnFailure_When_NoInvoiceFoundForPaymentReference()
        {
            var repo = new InvoiceRepository();

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            _payment = new PaymentBuilder();

            var result = paymentProcessor.ProcessPayment(_payment);
            result.ShouldFail("There is no invoice matching this payment");
        }

        [Test]
		public void ReturnFailure_When_NoPaymentNeeded( )
		{
			var repo = new InvoiceRepository( );

            _invoice = TestData.Invoice(0, 0);

			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

            _payment = new PaymentBuilder();

			var result = paymentProcessor.ProcessPayment(_payment);

			result.ShouldFail("no payment needed");
		}

		[Test]
		public void ReturnFailure_When_InvoiceAlreadyFullyPaid( )
		{
			var repo = new InvoiceRepository( );

            _invoice = TestData.Invoice(10, 10, 10);
			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

            _payment = new PaymentBuilder();

			var result = paymentProcessor.ProcessPayment(_payment);

			result.ShouldFail("invoice was already fully paid");
		}

		[Test]
		public void ReturnFailure_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var repo = new InvoiceRepository( );
            _invoice = TestData.Invoice(10, 5, 5);
			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			_payment = new PaymentBuilder().WithAmount(6);

			var result = paymentProcessor.ProcessPayment(_payment);

			result.ShouldFail("the payment is greater than the partial amount remaining");
		}

		[Test]
		public void ReturnFailure_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
            _invoice = TestData.Invoice(5, 0);
			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

            _payment = new PaymentBuilder().WithAmount(6);

			var result = paymentProcessor.ProcessPayment(_payment);

			result.ShouldFail("the payment is greater than the invoice amount");
		}

		[Test]
		public void ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var repo = new InvoiceRepository( );
            _invoice = TestData.Invoice(10, 5, 5);
			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

            _payment = new PaymentBuilder().WithAmount(5);

			var result = paymentProcessor.ProcessPayment(_payment);
			
			result.ShouldSucceed("final partial payment received, invoice is now fully paid");
		}

		[Test]
		public void ReturnFullyPaidMessage_When_InvoiceAlreadyPaid( )
		{
			var repo = new InvoiceRepository( );
            _invoice = TestData.Invoice(10, 0, 10);
			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

            _payment = new PaymentBuilder().WithAmount(10);

			var result = paymentProcessor.ProcessPayment(_payment);

			result.ShouldFail("invoice was already fully paid");
		}

		[Test]
		public void ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var repo = new InvoiceRepository( );
            _invoice = TestData.Invoice(10, 5, 5);
			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

            _payment = new PaymentBuilder().WithAmount(1);

			var result = paymentProcessor.ProcessPayment(_payment);

			result.ShouldSucceed("another partial payment received, still not fully paid");
		}

		[Test]
		public void ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
			_invoice = TestData.Invoice(10,0);
			repo.Add(_invoice);

			var paymentProcessor = new InvoicePaymentProcessor( repo );

            _payment = new PaymentBuilder().WithAmount(1);

			var result = paymentProcessor.ProcessPayment(_payment);

			result.ShouldSucceed("invoice is now partially paid");
		}


		// **********  ADDITIONAL TESTS FOR MISSING SCENARIOS **********
		[Test]
        public void ReturnFailure_When_ZeroAmountAndHasPayments()
        {
            var repo = new InvoiceRepository();

            _invoice = TestData.Invoice(0,0,5);

            repo.Add(_invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            _payment = new PaymentBuilder();

			var result = paymentProcessor.ProcessPayment(_payment);

            result.ShouldFail("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
        }

        [Test]
        public void ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var repo = new InvoiceRepository();
            _invoice = TestData.Invoice(5, 0);
            repo.Add(_invoice);

            var paymentProcessor = new InvoicePaymentProcessor(repo);

            _payment = new PaymentBuilder().WithAmount(5);

			var result = paymentProcessor.ProcessPayment(_payment);

            result.ShouldSucceed("invoice is now fully paid");
        }
    }
}