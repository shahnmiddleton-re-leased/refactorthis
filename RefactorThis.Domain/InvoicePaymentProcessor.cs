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
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            var responseMessage = string.Empty;

            if (inv.Amount == 0)
            {
                if (inv.Payments == null || !inv.Payments.Any())
                {
                    responseMessage = "no payment needed";
                }
                else
                {
                    throw new InvalidOperationException(
                        "The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }
            else
            {
                var pendingPayment = inv.PaymentPending();
                if (pendingPayment > 0M)
                {
                    if (payment.Amount > pendingPayment)
                    {
                        responseMessage = (payment.Amount > inv.Amount) ? "the payment is greater than the invoice amount" : "the payment is greater than the amount remaining";
                    }
                    else if(payment.Amount == pendingPayment)
                    {
                        inv.AddPayment(payment);
                        responseMessage = "invoice is now fully paid";
                    }
                    else
                    {
                        inv.AddPayment(payment);
                        responseMessage = "invoice is now partially paid";
                    }
                }
                else if (pendingPayment == 0M)
                {
                    responseMessage = "invoice was already fully paid";
                }

                inv.Save();
            }

            return responseMessage;
        }
    }
}