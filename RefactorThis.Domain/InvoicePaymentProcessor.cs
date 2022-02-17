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
			Invoice invoiceDetails = _invoiceRepository.GetInvoice( payment.invoiceNumber);

			String responseMessage;
            int totalPayments=invoiceDetails.Payments.Any( ) ? invoiceDetails.Payments.Sum( x => x.Amount ):0;
			int balancePayment=invoiceDetails.AmountDue-invoiceDetails.AmountPaid;
			if(invoiceDetails.AmountDue==0 || invoiceDetails.AmountDue==invoiceDetails.AmountPaid){
                responseMessage = "no payment needed or invoice was already fully paid";
            }
            else if (payment.Amount > balancePayment ){
               responseMessage = "the payment is greater than the remaining invoice amount";
            } else {
                if ( payment.Amount == balancePayment)  {
                                invoiceDetails.AmountPaid = invoiceDetails.AmountDue;
								responseMessage  = "invoice is now fully paid";
                 }
                else{
                                invoiceDetails.AmountPaid += payment.Amount;
								responseMessage = "another partial payment received, still not fully paid";
                    }
                invoiceDetails.Payments.Add( payment );
                _invoiceRepository.Save(invoiceDetails);
            }
               
			return responseMessage;
		}
	}
}
