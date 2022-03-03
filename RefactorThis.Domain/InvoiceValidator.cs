using RefactorThis.Persistence;
using System;
using System.Linq;

namespace RefactorThis.Domain
{
    public class InvoiceValidator
    {
        public InvoiceValidator() { }

        public void PreValidateInvoice(Invoice invoice)
        {
            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }
        }

        public bool DoesInvoiceRequireProcessing(Invoice invoice)
        {
            if (invoice.Amount > 0)
            {
                return true;
            }
            else
            {
                if (invoice.Payments != null && invoice.Payments.Any())
                {
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
                
                return false;
            }
        }

        public bool HasInvoiceBeenPaidInFull(Invoice invoice)
        {
            return invoice.Amount == invoice.AmountPaid;
        }

        public bool DoesPaymentExceedRemainingBalance(Invoice invoice, Payment payment)
        {
            return !HasInvoiceBeenPaidInFull(invoice) && invoice.RemainingBalance < payment.Amount;
        }

        public bool DoesInvoiceHavingExistingPayments(Invoice invoice)
        {
            return invoice.Payments != null && invoice.Payments.Any();
        }
    }
}
