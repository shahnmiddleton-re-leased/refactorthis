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

			string responseMessage;

			if (inv == null)
			{
				throw new InvalidOperationException("There is no invoice matching this payment");
			}
			else if(inv.Amount == 0)
            {
				responseMessage = "no payment needed";
			}
				
			else if (inv.Payments != null && inv.Payments.Any( ))
			{
				responseMessage = ProcessPartialPayment(inv, payment);
			}
			else
			{
				responseMessage = ProcessInitialPayment(inv, payment);
			}
				
			return responseMessage;
		}
	
		private void AddPayment(Invoice inv, Payment payment)
        {
			inv.AmountPaid = payment.Amount;
			inv.Payments.Add(payment);
			inv.Save();
		}

		private string ProcessInitialPayment(Invoice inv, Payment payment)
        {
			if (payment.Amount > inv.Amount)
			{
				return "the payment is greater than the invoice amount";
			}

			AddPayment(inv, payment);

			if (inv.Amount == payment.Amount)
			{
				return "invoice is now fully paid";
			}
			else
			{
				return "invoice is now partially paid";
			}
		}

		private string ProcessPartialPayment(Invoice inv, Payment payment)
        {
			decimal balanceAmount = inv.Amount - inv.AmountPaid;

			if (balanceAmount == 0)
			{
				return "invoice was already fully paid";
			}
			else if (payment.Amount > balanceAmount)
			{
				return "the payment is greater than the partial amount remaining";
			}

			AddPayment(inv, payment);

			if (balanceAmount == payment.Amount)
			{
				return "final partial payment received, invoice is now fully paid";
			}
			else
			{
				return "another partial payment received, still not fully paid";
			}
			
		}
	}
}