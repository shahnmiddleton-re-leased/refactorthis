using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence;
using Unity;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		private static UnityContainer unityContainer = new UnityContainer();
		private readonly IInventoryRepository _repo = null;
		private readonly IModelFactory _factory = null;

		static InvoicePaymentProcessorTests()
        {
			unityContainer.RegisterType<IInventoryRepository, InvoiceRepository>();
			unityContainer.RegisterType<IModelFactory, ModelFactory>();
		}

		public InvoicePaymentProcessorTests(IInventoryRepository inventoryRepository, IModelFactory modelFactory)
        {
			_repo = inventoryRepository;
			_factory = modelFactory;
        }

		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
			var invoice = _factory.GetInvoiceObject(_repo);
			var paymentProcessor =_factory.GetInvoicePaymentProcessorObject( _repo );

			var payment = _factory.GetPaymentObject();
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
			var invoice = _factory.GetPopulatedInvoiceObject(_repo, null, 0, 0);			

			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject(_repo);

			var payment = _factory.GetPaymentObject();

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "no payment needed", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{

			var payment = _factory.GetPopulatedPaymentObject(10);

			var payments = _factory.GetPopulatedPaymentsObject(payment);


			var invoice = _factory.GetPopulatedInvoiceObject(_repo, payments, 10, 10);

			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject( _repo );

			//var payment = new Payment( );
			payment = _factory.GetPaymentObject();

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var payment = _factory.GetPopulatedPaymentObject(5);

			var payments = _factory.GetPopulatedPaymentsObject(payment);


			var invoice = _factory.GetPopulatedInvoiceObject(_repo, payments, 10, 5);


			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject(_repo);

			payment = _factory.GetPopulatedPaymentObject(6);

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the partial amount remaining", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var payment = _factory.GetPopulatedPaymentObject(5);

			var payments = _factory.GetPopulatedPaymentsObject();


			var invoice = _factory.GetPopulatedInvoiceObject(_repo, payments, 5, 0);

			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject(_repo);

			payment = _factory.GetPopulatedPaymentObject(6);

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the invoice amount", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var payment = _factory.GetPopulatedPaymentObject(5);

			var payments = _factory.GetPopulatedPaymentsObject();


			var invoice = _factory.GetPopulatedInvoiceObject(_repo, payments, 10, 5);

			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject(_repo);

			payment = _factory.GetPopulatedPaymentObject(6);

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "final partial payment received, invoice is now fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			var payment = _factory.GetPopulatedPaymentObject(10);

			var payments = _factory.GetPopulatedPaymentsObject();


			var invoice = _factory.GetPopulatedInvoiceObject(_repo, payments, 10, 0);

			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject(_repo);

			payment = _factory.GetPopulatedPaymentObject(10);

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var payment = _factory.GetPopulatedPaymentObject(5);

			var payments = _factory.GetPopulatedPaymentsObject();


			var invoice = _factory.GetPopulatedInvoiceObject(_repo, payments, 10, 5);

			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject(_repo);

			payment = _factory.GetPopulatedPaymentObject(1);

			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "another partial payment received, still not fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var payment = _factory.GetPopulatedPaymentObject(1);

			var payments = _factory.GetPopulatedPaymentsObject();


			var invoice = _factory.GetPopulatedInvoiceObject(_repo, payments, 10, 0);

			_repo.Add( invoice );

			var paymentProcessor = _factory.GetInvoicePaymentProcessorObject(_repo);


			var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice is now partially paid", result );
		}
	}
}