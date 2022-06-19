using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            var validationMessage = Validate(inv, payment);
            if (validationMessage != null) return validationMessage;

            var responseMessage = inv.ProcessPayment(payment);

            return responseMessage;
        }

        private static string Validate(Invoice inv, Payment payment)
        {
            ValidateInvoice(inv);
            return ValidatePayment(inv, payment);
        }

        private static void ValidateInvoice(Invoice inv)
        {
            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            if (inv.Amount == 0 && inv.Payments != null && inv.Payments.Any())
            {
                throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }
        }

        private static string ValidatePayment(Invoice inv, Payment payment)
        {
            if (inv.Amount == 0 && (inv.Payments == null || !inv.Payments.Any()))
            {
                return "no payment needed";
            }

            if (payment.Amount > inv.Amount)
            {
                return "the payment is greater than the invoice amount";
            }

            var amountPaid = inv.AmountPaid();

            if (amountPaid != 0 && inv.Amount == amountPaid)
            {
                return "invoice was already fully paid";
            }

            if (amountPaid != 0 && payment.Amount > (inv.Amount - amountPaid))
            {
                return "the payment is greater than the partial amount remaining";
            }
            return null;
        }
    }
}