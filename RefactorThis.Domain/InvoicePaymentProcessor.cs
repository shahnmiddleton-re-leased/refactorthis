using System;
using System.Linq;
using RefactorThis.Domain.Properties;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor : IInvoicePaymentProcessor
	{
		private readonly IInvoiceRepository _invoiceRepository;
		public InvoicePaymentProcessor(IInvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		/// <summary>
		/// Validate and Process payment
		/// </summary>
		/// <param name="payment"></param>
		/// <returns>Returns user message based on payment processed</returns>
		public string ProcessPayment(Payment payment)
		{
			var inv = _invoiceRepository.GetInvoice(payment.Reference);

			if (inv == null)
				throw new InvalidOperationException(Resources.NoMatchingInvoice);

			if (inv.Amount == 0)
			{
				if (inv.Amount == 0 && (inv.Payments == null || inv.Payments.Count == 0))
				{
					return Resources.NoPaymentNeeded;
				}
				else
					throw new InvalidOperationException(Resources.InvoiceInInvalidState);
			}

			var responseMessage = ProcessPayment(inv, payment);
			return responseMessage;
		}

		private string ProcessPayment(Invoice inv, Payment payment)
		{
			try
			{
				string responseMessage;

				decimal invoicePayment = inv.Payments.Sum(x => x.Amount);
				decimal invoiceAmount = inv.Amount;
				decimal invoiceFinalAmount = invoiceAmount - inv.AmountPaid;
				bool partPaymentExists = invoicePayment > 0;

				if (invoiceAmount == invoicePayment)
					return Resources.InvoiceAlreadyPaid;

				if (payment.Amount <= invoiceFinalAmount)
				{
					// Paid amount is equal to remaining amount
					if (invoiceFinalAmount == payment.Amount)
					{
						inv.AmountPaid += payment.Amount;
						inv.Payments.Add(payment);
						responseMessage = partPaymentExists ? Resources.FinalPartialPaymentInvoiceFullyPaid : Resources.InvoiceFullyPaid;
					}
					// Paid amount is less than remaining amount
					else
					{
						inv.AmountPaid += payment.Amount;
						inv.Payments.Add(payment);
						responseMessage = partPaymentExists ? Resources.PartialPaymentInvoiceNotFullyPaid : Resources.InvoicePartialPaid;
					}
				}

				// Paid amount is more than remaining amount
				else
					responseMessage = partPaymentExists ? Resources.OverpaidPartialPayment : Resources.PaymentOverpaid;

				_invoiceRepository.SaveInvoice(inv);
				return responseMessage;

			}
			catch (Exception e)
			{
				// Write to log e.message
				return Resources.ErrorMessage;
			}
		}
	}
}