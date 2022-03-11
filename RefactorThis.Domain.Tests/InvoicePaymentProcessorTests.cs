using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
			var paymentProcessor = new InvoicePaymentProcessor();
			const string refKey = "INVOICE000";

			var payment = new Payment( )
			{
				Amount = 5,
				Reference = refKey
			};
			var failureMessage = "";

			try
			{
				var result = paymentProcessor.ProcessPayment( payment );
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual( "There is no invoice matching this payment", failureMessage );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE001";

			var invoice = new Invoice( )
			{
				Amount = 0
			};

			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor( );
			
			var payment = new Payment( )
			{
				Amount = 5,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "no payment needed", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE002";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			invoice.AddPayment(new Payment
			{
				Amount = 10,
				Reference = refKey
			});
			
			repo.Add( refKey,invoice );

			var paymentProcessor = new InvoicePaymentProcessor();

			var payment = new Payment( )
			{
				Amount = 5,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE003";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			invoice.AddPayment(new Payment
			{
				Amount = 5,
				Reference = refKey
			});
			
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();

			var payment = new Payment( )
			{
				Amount = 6,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the partial amount remaining", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE004";
			
			var invoice = new Invoice()
			{
				Amount = 5
			};
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();

			var payment = new Payment( )
			{
				Amount = 6,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the invoice amount", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE006";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			
			invoice.AddPayment(new Payment
			{
				Amount = 5
			});
			
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();

			var payment = new Payment( )
			{
				Amount = 5,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "final partial payment received, invoice is now fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE007";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			invoice.AddPayment(new Payment() {Amount = 10});
			
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();

			var payment = new Payment( )
			{
				Amount = 10,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE008";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			invoice.AddPayment(new Payment
			{
				Amount = 5
			});
			
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();

			var payment = new Payment( )
			{
				Amount = 1,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "another partial payment received, still not fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE009";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();

			var payment = new Payment( )
			{
				Amount = 1,
				Reference = refKey
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice is now partially paid", result );
		}
		
		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NullIsPassedAsThePaymentObject( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE011";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();
			
			var result = paymentProcessor.ProcessPayment( null );

			Assert.AreEqual( "payment is null", result );
		}
		
		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PaymentObjectWithAmountZeroIsAdded( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE012";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();
			
			var payment = new Payment( )
			{
				Amount = 0,
				Reference = refKey
			};
			
			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "payment amount in invalid", result );
		}
		
		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PaymentObjectWithNegativeAmountIsAdded( )
		{
			var repo = InvoiceRepository.GetInvoiceRepository();
			const string refKey = "INVOICE013";
			
			var invoice = new Invoice()
			{
				Amount = 10
			};
			repo.Add( refKey, invoice );

			var paymentProcessor = new InvoicePaymentProcessor();
			
			var payment = new Payment( )
			{
				Amount = -5,
				Reference = refKey
			};
			
			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "payment amount in invalid", result );
		}
	}
}