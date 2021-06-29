using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor
	{
		private readonly InvoiceRepository _invoiceRepository;

		public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment(Payment payment)
		{
			var inv = _invoiceRepository.GetInvoice(payment.Reference);

			if (inv == null)
			{
				throw new InvalidOperationException("There is no invoice matching this payment");
			}
			if (inv.Amount == 0 && inv.Payments != null && inv.Payments.Count > 0)
			{
				throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
			}
			if (inv.Amount == 0)
			{
				return "no payment needed";
			}
			string responseMessage;
			if (inv.Payments != null && inv.Payments.Any())
			{
				responseMessage = ProcessPaymentForPartiallyPaidInvoice(inv, payment);
			}
			else
			{
				responseMessage = ProcessNewPayment(inv, payment);
			}

			return responseMessage;
		}

		#region Private methods
		private string ProcessNewPayment(Invoice inv, Payment payment)
		{
			if (payment.Amount > inv.Amount)
			{
				return "the payment is greater than the invoice amount";
			}
			AddInvoicePayment(inv, payment);
			if (inv.Amount == payment.Amount)
			{
				return "invoice is now fully paid";
			}
			else
			{
				return "invoice is now partially paid";
			}
		}

		private string ProcessPaymentForPartiallyPaidInvoice(Invoice inv, Payment payment)
		{
			decimal amountPaid = inv.Payments.Sum(p => p.Amount);
			decimal paymentDue = inv.Amount - amountPaid;
			if (paymentDue == 0)
			{
				return "invoice was already fully paid";
			}
			if (payment.Amount > paymentDue)
			{
				return "the payment is greater than the partial amount remaining";
			}
			AddInvoicePayment(inv, payment);
			if (paymentDue == payment.Amount)
			{
				return "final partial payment received, invoice is now fully paid";
			}
			else
			{
				return "another partial payment received, still not fully paid";
			}

		}

		private void AddInvoicePayment(Invoice inv, Payment payment)
		{
			inv.AmountPaid += payment.Amount;
			inv.Payments.Add(payment);
			inv.Save();
		}

		#endregion
	}
}