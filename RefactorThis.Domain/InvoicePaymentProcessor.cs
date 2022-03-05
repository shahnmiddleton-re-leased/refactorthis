using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor
	{
		private readonly InvoiceRepository _invoiceRepository;

		public InvoicePaymentProcessor( InvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment( Payment payment )
		{
			var inv = _invoiceRepository.GetInvoice( payment.Reference );

			var responseMessage = string.Empty;

			if ( inv == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}
			
			if ( inv.Amount == 0)
			{
				if (!PaymentMade(inv))
				{
					return "no payment needed";
				}
				
				throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
			}
			
			if (PaymentMade(inv))
			{
				if (inv.Amount == inv.Payments.Sum( x => x.Amount ) )
				{
					return "invoice was already fully paid";
				}

				if (payment.Amount > ( inv.Amount - inv.AmountPaid ) )
				{
					return "the payment is greater than the partial amount remaining";
				}
				
				if ( ( inv.Amount - inv.AmountPaid ) == payment.Amount )
				{
					responseMessage = "final partial payment received, invoice is now fully paid";
				}
				else
				{
					responseMessage = "another partial payment received, still not fully paid";
				}
				AddNewPayment(inv, payment);
			}
			else
			{
				if ( payment.Amount > inv.Amount )
				{
					return "the payment is greater than the invoice amount";
				}
				
				if ( inv.Amount == payment.Amount )
				{
					responseMessage = "invoice is now fully paid";
				}
				else
				{
					responseMessage = "invoice is now partially paid";
				}
				AddNewPayment(inv, payment);
			}
			
			inv.Save();

			return responseMessage;
		}

		private void AddNewPayment(Invoice inv, Payment payment)
        {
			inv.AmountPaid += payment.Amount;
			inv.Payments.Add(payment);
		}

		private bool PaymentMade(Invoice inv)
        {
			if (inv.Payments == null || !inv.Payments.Any())
            {
				return false;
			}
			return true;
		}
	}
}