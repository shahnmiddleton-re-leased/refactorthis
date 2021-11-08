using NUnit.Framework;
using RefactorThis.Invoicing.Domain.Services;
using RefactorThis.Invoicing.Domain.Interfaces;
using RefactorThis.Invoicing.Domain.Model;
using System.Linq;
using System.Collections.Generic;

namespace RefactorThis.Tests.Invoicing
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		private IInvoicePaymentProcessor _invoicePaymentProcessor;

		[SetUp]
        public void Setup()
        {
			_invoicePaymentProcessor = new InvoicePaymentProcessor();
		}

		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
		{
			
			Invoice invoice = null;			
			var payment = new Payment();

			_invoicePaymentProcessor.AddInvoice(invoice);

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			// There is no invoice matching this payment
			Assert.True(result.Validations.Any(x => x.Code == "INV00001"));
		}


		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
		{			
			var invoice = new Invoice
			{
				Amount = 0,
				AmountPaid = 0,
				Payments = null
			};

			_invoicePaymentProcessor.AddInvoice(invoice);

			var payment = new Payment();

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			// No payment needed
			Assert.True(result.Validations.Any(x => x.Code == "INV00002"));
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
					new Payment
					{
						Amount = 10
					}
				}
			};
			
			_invoicePaymentProcessor.AddInvoice(invoice);

			var payment = new Payment();

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			// Invoice was already fully paid
			Assert.True(result.Validations.Any(x => x.Code == "INV00004"));
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
					new Payment
					{
						Amount = 5
					}
				}
			};

			_invoicePaymentProcessor.AddInvoice(invoice);

			var payment = new Payment()
			{
				Amount = 6
			};

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			// The payment is greater than the partial amount remaining
			Assert.True(result.Validations.Any(x => x.Code == "INV00005"));
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

			_invoicePaymentProcessor.AddInvoice(invoice);

			var payment = new Payment()
			{
				Amount = 6
			};

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			// The payment is greater than the invoice amount
			Assert.True(result.Validations.Any(x => x.Code == "INV00006"));
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

			_invoicePaymentProcessor.AddInvoice(invoice);
			
			var payment = new Payment()
			{
				Amount = 5
			};

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			Assert.True(result.Information.Any(x => x == "final partial payment received, invoice is now fully paid"));			
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
		{
			var invoice = new Invoice
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>() { new Payment() { Amount = 10 } }
			};

			_invoicePaymentProcessor.AddInvoice(invoice);

			var payment = new Payment()
			{
				Amount = 10
			};

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			// Invoice was already fully paid
			Assert.True(result.Validations.Any(x => x.Code == "INV00004"));
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
					new Payment
					{
						Amount = 5
					}
				}
			};

			_invoicePaymentProcessor.AddInvoice(invoice);

			var payment = new Payment()
			{
				Amount = 1
			};

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			Assert.True(result.Information.Any(x => x == "another partial payment received, still not fully paid"));			
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

			_invoicePaymentProcessor.AddInvoice(invoice);
			
			var payment = new Payment()
			{
				Amount = 1
			};

			var result = _invoicePaymentProcessor.ProcessPayment(payment);

			Assert.True(result.Information.Any(x => x == "invoice is now partially paid"));
		}
	}
}