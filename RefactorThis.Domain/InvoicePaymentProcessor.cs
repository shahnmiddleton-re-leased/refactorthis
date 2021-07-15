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
		/// <summary>
		/// Process payment
		/// </summary>
		/// <param name="payment"></param>
		/// <returns></returns>
		public string ProcessPayment(Payment payment)
		{
			var inv = Check_Invoice(_invoiceRepository.GetInvoice(payment.Reference));

			if (inv.Amount == 0 && inv.Payments != null && inv.Payments.Any())
				throw new InvalidOperationException(ErrorMessages.DATA_INTEGRITY_INVALID_INVOICE_AMOUNT_0);

			return Check_Invoice_Payment_Processing(inv, payment);

		}

		#region Invoice Processing methods
	
		private Invoice Check_Invoice(Invoice inv)
		{
			if (inv == null)
				throw new InvalidOperationException(ErrorMessages.DATA_INTEGRITY_NO_INVOICE_MATCH);
			else
				return inv;
		}
		
		private string Check_Invoice_Payment_Processing(Invoice invoice, Payment payment)
		{
			string message = "";
			if(invoice.Amount== 0)
				message = ResponseMessages.INVOICE_NO_PAYMENT_NEEDED;
			else if (invoice.Payments != null && invoice.Payments.Any())
				CheckPartialPayment(invoice, payment);
			else
				CheckOtherPayment(invoice, payment);
			return message;
		}
	
		private string CheckPartialPayment(Invoice invoice, Payment payment)
        {
			string message = "";
			if (invoice.Payments.Sum(x => x.Amount) == invoice.Amount)
				message = ResponseMessages.INVOICE_ALREADY_PAID;
			else if (payment.Amount > (invoice.Amount - invoice.Payments.Sum(x => x.Amount)))
				message = ResponseMessages.INVOICE_PAID_GREATER_THEN_REMAINING;
			else if ((invoice.Amount - invoice.AmountPaid) == payment.Amount)
			{
				UpdateInvoicePayment(invoice, payment);
				message = ResponseMessages.INVOICE_AMOUNT_PARTIAL_PAYMENT_FULLY_PAID;
			}
			else
			{
				UpdateInvoicePayment(invoice, payment);
				message = ResponseMessages.INVOICE_AMOUNT_PAID_PARTIALLY;
			}

			return message;
		}

		private string CheckOtherPayment(Invoice invoice, Payment payment)
        {
			string message = "";
			if (payment.Amount > invoice.Amount)
				message = ResponseMessages.INVOICE_PAID_GREATER_THEN_TOTAL;
			else if (invoice.Amount == payment.Amount)
			{
				UpdateInvoicePayment(invoice, payment);
				message = ResponseMessages.INVOICE_PAID_FULLY;
			}
			else
			{
				UpdateInvoicePayment(invoice, payment);
				message = ResponseMessages.INVOICE_PAID_PARTIALLY;
			}
			return message;
		}
		
		private void UpdateInvoicePayment(Invoice inv, Payment payment)
		{
			inv.AmountPaid += payment.Amount;
			inv.Payments.Add(payment);
			inv.Save();
		}

		
		#endregion

	}
}