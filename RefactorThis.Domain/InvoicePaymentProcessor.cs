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

			string responseMessage;

			if ( inv == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}

            if ( inv.Amount == 0 )
            {
                responseMessage = NoPaymentRequired(inv);
            }
            else if (FirstPayment(inv))
            {
                responseMessage = ProcessFirstPayment(payment, inv);
            }
            else
            {
                responseMessage = ProcessAdditionalPayment(payment, inv);
            }

            inv.Save();

			return responseMessage;
		}

        private static bool FirstPayment(Invoice inv)
        {
            return inv.Payments == null || !inv.Payments.Any();
        }

        private static string ProcessFirstPayment(Payment payment, Invoice inv)
        {
            string responseMessage;
            if (payment.Amount > inv.Amount)
            {
                responseMessage = "the payment is greater than the invoice amount";
            }
            else if (inv.Amount == payment.Amount)
            {
                AddPayment(payment, inv);
                responseMessage = "invoice is now fully paid";
            }
            else
            {
                AddPayment(payment, inv);
                responseMessage = "invoice is now partially paid";
            }

            return responseMessage;
        }

        private static string ProcessAdditionalPayment(Payment payment, Invoice inv)
        {
            string responseMessage;
            if (InvoiceIsPaid(inv))
            {
                responseMessage = "invoice was already fully paid";
            }
            else if (PaymentGreaterThanAmountRemaining(payment, inv))
            {
                responseMessage = "the payment is greater than the partial amount remaining";
            }
            else if(IsFinalPayment(payment, inv))
            {
                AddPayment(payment, inv);
                responseMessage = "final partial payment received, invoice is now fully paid";
            }
            else
            {
                AddPayment(payment, inv);
                responseMessage = "another partial payment received, still not fully paid";
            }

            return responseMessage;
        }

        private static void AddPayment(Payment payment, Invoice inv)
        {
            inv.AmountPaid += payment.Amount;
            inv.Payments.Add(payment);
        }

        private static bool IsFinalPayment(Payment payment, Invoice inv)
        {
            return (inv.Amount - inv.AmountPaid) == payment.Amount;
        }

        private static bool PaymentGreaterThanAmountRemaining(Payment payment, Invoice inv)
        {
            return inv.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (inv.Amount - inv.AmountPaid);
        }

        private static bool InvoiceIsPaid(Invoice inv)
        {
            return inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount);
        }

        private static string NoPaymentRequired(Invoice inv)
        {
            if (inv.Payments != null && inv.Payments.Any())
            {
                throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }

            return "no payment needed";
        }
    }
}