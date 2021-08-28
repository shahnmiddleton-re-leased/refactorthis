using System;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor : IInvoicePaymentProcessor
    {
		private readonly IInvoiceService _invoiceService;

        public InvoicePaymentProcessor( IInvoiceService invoiceService )
        {
            _invoiceService = invoiceService;
        }

		public string ProcessPayment( Payment payment )
		{
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment), "Must have a valid payment to add");
            }
            
            var inv = _invoiceService.GetInvoice( payment.Reference );

            if ( inv == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}

            var responseMessage = _invoiceService.AddPaymentToInvoice(inv, payment);

            _invoiceService.SaveInvoice(inv);
            return responseMessage;
		}
	}
}