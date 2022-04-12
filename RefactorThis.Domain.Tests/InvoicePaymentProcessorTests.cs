using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessor_Should
	{
		[Test]
		public void ThrowException_When_NoInvoiceFoundForPaymentReference( )
		{
			var repo = new InvoiceRepository( );

			Invoice invoice = null;
			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( );

            Action act = () => paymentProcessor.ProcessPayment(payment);

			act.Should().Throw<InvalidOperationException>()
                .WithMessage("There is no invoice matching this payment");
		}

		[Test]
		public void ReturnFailureMessage_When_NoPaymentNeeded( )
		{
			var repo = new InvoiceRepository( );

			var invoice = new Invoice( repo )
			{
				Amount = 0,
				AmountPaid = 0,
				Payments = null
			};

			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( );

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "no payment needed");
		}

		[Test]
		public void ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var repo = new InvoiceRepository( );

			var invoice = new Invoice( repo )
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
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( );

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "invoice was already fully paid");
		}

		[Test]
		public void ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
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
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "the payment is greater than the partial amount remaining");
		}

		[Test]
		public void ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
			{
				Amount = 5,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "the payment is greater than the invoice amount");
		}

		[Test]
		public void ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
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
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( )
			{
				Amount = 5
			};

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "final partial payment received, invoice is now fully paid");
		}

		[Test]
		public void ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( ) { new Payment( ) { Amount = 10 } }
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( )
			{
				Amount = 10
			};

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "invoice was already fully paid");
		}

		[Test]
		public void ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
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
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "another partial payment received, still not fully paid");
		}

		[Test]
		public void ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoicePaymentProcessor( repo );

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment( payment );

			result.Should().Be( "invoice is now partially paid");
		}
	}
}