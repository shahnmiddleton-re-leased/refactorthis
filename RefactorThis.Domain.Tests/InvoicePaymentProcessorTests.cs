using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence;
using RefactorThis.Persistence.Classes;
using RefactorThis.Persistence.Model;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
			var financialsService = new FinancialsService(new AccountsRepository()); //Object will be created/intentiate through Dependency Injection.

			Invoice invoice = null;  
			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( );
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
			var financialsService = new FinancialsService(new AccountsRepository());

			var invoice = new Invoice
			{
				Amount = 0,
				AmountPaid = 0,
				Payments = null
			};

			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( );

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "no payment needed", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var financialsService = new FinancialsService(new AccountsRepository());

			var invoice = new Invoice()
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
			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( );

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var financialsService = new FinancialsService(new AccountsRepository());
			var invoice = new Invoice()
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
			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the partial amount remaining", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var financialsService = new FinancialsService(new AccountsRepository());
			var invoice = new Invoice()
			{
				Amount = 5,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the invoice amount", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var financialsService = new FinancialsService(new AccountsRepository());
			var invoice = new Invoice()
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
			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( )
			{
				Amount = 5
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "final partial payment received, invoice is now fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			var financialsService = new FinancialsService(new AccountsRepository());
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( ) { new Payment( ) { Amount = 10 } }
			};
			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( )
			{
				Amount = 10
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var financialsService = new FinancialsService(new AccountsRepository());
			var invoice = new Invoice()
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
			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "another partial payment received, still not fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var financialsService = new FinancialsService(new AccountsRepository());
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
			financialsService.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor(financialsService);

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice is now partially paid", result );
		}
	}
}