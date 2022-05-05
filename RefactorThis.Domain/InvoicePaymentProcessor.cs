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

			responseMessage = inv.AddPayment(payment);			
			
			inv.Save();

			return responseMessage;
		}
	}
}