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

		/// <summary>
		/// Validate the invoice we're processing, throwing any appropriate exceptions if invoice is not in a valid state.
		/// </summary>
		/// <param name="invoice">Invoice to validate, may be null.</param>
		private void Validate( Invoice invoice )
		{
			if ( invoice == null)
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}

			if ( invoice.Amount == 0 && invoice.HasPayments() )
			{
				throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
			}
		}

		/// <summary>
		/// Given a valid invoice, is payment req'd on this invoice?
		/// </summary>
		/// <param name="invoice"></param>
		/// <returns>true if payment processing is req'd, otherwise false.</returns>
		private bool IsPaymentRequired( Invoice invoice, Payment payment, out string message )
        {
			bool paymentReqd = true;
			message = string.Empty;

			if ( invoice.Amount == 0 )
			{
				if ( !invoice.HasPayments() )
				{
					message = "no payment needed";
					paymentReqd = false;
				}
			}
			else
			{
				if ( invoice.HasPayments() )
				{
					var totalPayments = invoice.TotalPayments();
					if ( totalPayments != 0 )
					{
						if ( invoice.Amount == totalPayments )
						{
							message = "invoice was already fully paid";
							paymentReqd = false;
						}
						else if ( payment.Amount > invoice.AmountOutstanding )
						{
							message = "the payment is greater than the partial amount remaining";
							paymentReqd = false;
						}
					}
				}
			}

			return paymentReqd;
        }

		public string ProcessPayment( Payment payment )
		{
			var responseMessage = string.Empty;
			var invoice = _invoiceRepository.GetInvoice( payment.Reference );

			// Will throw if the invoice is in an invalid state.
			Validate( invoice ); 

			if ( IsPaymentRequired( invoice, payment, out responseMessage ) )
			{
				if ( invoice.HasPayments() )
				{
					if ( invoice.AmountOutstanding == payment.Amount )
					{
						responseMessage = "final partial payment received, invoice is now fully paid";
					}
					else
					{
						responseMessage = "another partial payment received, still not fully paid";
					}

					invoice.MakePayment(payment);
				}
				else
				{
					if (payment.Amount > invoice.Amount)
					{
						responseMessage = "the payment is greater than the invoice amount";
					}
					else
					{
						if (invoice.Amount == payment.Amount)
						{
							responseMessage = "invoice is now fully paid";
						}
						else
						{
							responseMessage = "invoice is now partially paid";
						}

						invoice.MakePayment(payment);
					}
				}
			}

			return responseMessage;
		}
	}
}