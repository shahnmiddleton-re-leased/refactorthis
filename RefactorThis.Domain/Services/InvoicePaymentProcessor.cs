using FluentResults;
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

		public Result ProcessPayment( Payment payment )
		{
			var invoice = _invoiceRepository.GetInvoice( payment.Reference );

			if ( invoice == null )
			{
				return Result.Fail("There is no invoice matching this payment" );
			}

            var result = invoice.AddPayment(payment);

            if (result.IsSuccess)
            {
                _invoiceRepository.SaveInvoice(invoice);
            }

			return result;
		}
	}
}