using NUnit.Framework;
using RefactorThis.Domain.Model.Invoices;
using RefactorThis.Domain.Services;
using RefactorThis.Domain.Tests.TestHelpers;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
    public class InvoicePaymentProcessor_Should : SystemUnderTestIs<InvoicePaymentProcessor>
    {
        private Payment _payment = new PaymentBuilder();
        private Invoice _invoice;

		[Test]
        public void ReturnFailure_When_NoInvoiceFoundForPaymentReference()
        {
            SUT.ProcessPayment(_payment)
                .ShouldFail("There is no invoice matching this payment");
        }

        [Test]
		public void ReturnFailure_When_NoPaymentNeeded( )
		{
            _invoice = TestData.Invoice(0, 0);
            SUT.ProcessPayment(_payment)
                .ShouldFail("no payment needed");
        }

		[Test]
		public void ReturnFailure_When_InvoiceAlreadyFullyPaid( )
		{
            _invoice = TestData.Invoice(10, 10, 10);
			SUT.ProcessPayment(_payment)
                .ShouldFail("invoice was already fully paid");
		}

		[Test]
		public void ReturnFailure_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
            _invoice = TestData.Invoice(10, 5, 5);
			_payment = new PaymentBuilder().WithAmount(6);
            SUT.ProcessPayment(_payment)
                .ShouldFail("the payment is greater than the partial amount remaining");
        }

		[Test]
		public void ReturnFailure_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
            _invoice = TestData.Invoice(5, 0);
            _payment = new PaymentBuilder().WithAmount(6);
            SUT.ProcessPayment(_payment)
                .ShouldFail("the payment is greater than the invoice amount");
		}

		[Test]
		public void ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
            _invoice = TestData.Invoice(10, 5, 5);
            _payment = new PaymentBuilder().WithAmount(5);
            SUT.ProcessPayment(_payment)
                .ShouldSucceed("final partial payment received, invoice is now fully paid");
        }

		[Test]
		public void ReturnFullyPaidMessage_When_InvoiceAlreadyPaid( )
		{
            _invoice = TestData.Invoice(10, 0, 10);
            _payment = new PaymentBuilder().WithAmount(10);
            SUT.ProcessPayment(_payment)
                .ShouldFail("invoice was already fully paid");
        }

		[Test]
		public void ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
            _invoice = TestData.Invoice(10, 5, 5);
            _payment = new PaymentBuilder().WithAmount(1);
            SUT.ProcessPayment(_payment)
                .ShouldSucceed("another partial payment received, still not fully paid");
        }

		[Test]
		public void ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			_invoice = TestData.Invoice(10,0);
            _payment = new PaymentBuilder().WithAmount(1);
            SUT.ProcessPayment(_payment)
                .ShouldSucceed("invoice is now partially paid");
        }
        
		// **********  ADDITIONAL TESTS FOR MISSING SCENARIOS **********
		[Test]
        public void ReturnFailure_When_ZeroAmountAndHasPayments()
        {
            _invoice = TestData.Invoice(0,0,5);
			SUT.ProcessPayment(_payment)
                .ShouldFail("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
        }

        [Test]
        public void ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            _invoice = TestData.Invoice(5, 0);
            _payment = new PaymentBuilder().WithAmount(5);
			SUT.ProcessPayment(_payment)
                .ShouldSucceed("invoice is now fully paid");
        }

        protected override void Initialize()
        {
            _payment = new PaymentBuilder();
            _invoice = null;
        }

        protected override InvoicePaymentProcessor CreateSut()
        {
            var repository = new InvoiceRepository();
            repository.Add(_invoice);
            return new InvoicePaymentProcessor(repository);
        }
    }
}