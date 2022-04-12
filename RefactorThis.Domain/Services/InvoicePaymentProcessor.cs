using System;
using System.Linq;
using RefactorThis.Domain.Model;
using RefactorThis.Domain.Repositories;

namespace RefactorThis.Domain.Services
{
	public class InvoicePaymentProcessor
	{
		private readonly IInvoiceRepository _invoiceRepository;

		public InvoicePaymentProcessor(IInvoiceRepository invoiceRepository )
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

            var responseMessage = invoice.AddPayment(payment);

            _invoiceRepository.SaveInvoice(invoice);

			return responseMessage;
		}
	}
}