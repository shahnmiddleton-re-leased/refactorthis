using System;
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
			var invoice = _invoiceRepository.GetInvoice( payment.Reference );

			if ( invoice == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}

			return invoice.AddPayment( payment );
		}
	}
}