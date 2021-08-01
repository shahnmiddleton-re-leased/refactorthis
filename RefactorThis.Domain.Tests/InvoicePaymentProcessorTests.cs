using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using RefactorThis.Domain.Services;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		private IInvoicePaymentService service;
		private InvoiceRepository repo;
		private Payment payment;

		[SetUp]
		public void TestStarting()
        {
			repo = new InvoiceRepository();
			service = new InvoicePaymentService(repo);
			payment = new Payment();
        }

		[Test]
		public async Task ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
		{
			var failureMessage = "";

			try
			{
				var result = await service.ProcessPayment(payment);
			}
			catch (InvalidOperationException e)
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual("There is no invoice matching this payment.", failureMessage);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
		{
			var invoice = new Invoice(repo)
			{
				Amount = 0,
				AmountPaid = 0,
				Payments = null
			};

			repo.Add(invoice);

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("No payment needed.", result);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
		{
			var invoice = new Invoice(repo)
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
			repo.Add(invoice);

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("Invoice was already fully paid.", result);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
		{
			var invoice = new Invoice(repo)
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
			repo.Add(invoice);

			var payment = new Payment()
			{
				Amount = 6
			};

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("The payment is greater than the partial amount remaining.", result);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
		{
			var invoice = new Invoice(repo)
			{
				Amount = 5,
				AmountPaid = 0,
				Payments = new List<Payment>()
			};
			repo.Add( invoice );

			var payment = new Payment()
			{
				Amount = 6
			};

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("The payment is greater than the invoice amount.", result);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
		{
			var invoice = new Invoice(repo)
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
			repo.Add(invoice);

			var payment = new Payment()
			{
				Amount = 5
			};

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("Final partial payment received, invoice is now fully paid.", result);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
		{
			var invoice = new Invoice(repo)
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>() { new Payment() { Amount = 10 } }
			};
			repo.Add(invoice);

			var payment = new Payment()
			{
				Amount = 10
			};

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("Invoice was already fully paid.", result);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
		{
			var invoice = new Invoice(repo)
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
			repo.Add(invoice);

			var payment = new Payment()
			{
				Amount = 1
			};

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("Another partial payment received, still not fully paid.", result);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
		{
			var invoice = new Invoice(repo)
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>()
			};
			repo.Add(invoice);

			var payment = new Payment()
			{
				Amount = 1
			};

			var result = await service.ProcessPayment(payment);

			Assert.AreEqual("Invoice is now partially paid.", result);
		}
	}
}