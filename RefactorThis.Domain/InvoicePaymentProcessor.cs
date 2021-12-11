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

            string responseMessage;

            if (inv.Amount > 0)
            {
                var pendingPayment = inv.PaymentPending();
                if (pendingPayment > 0)
                {
                    if (payment.Amount > pendingPayment)
                    {
                        responseMessage = (payment.Amount > inv.Amount) ? "the payment is greater than the invoice amount" : "the payment is greater than the amount remaining";
                    }
                    else
                    {
                        inv.AddPayment(payment);
                        responseMessage = (payment.Amount == pendingPayment) ? "invoice is now fully paid" : "invoice is now partially paid";
                    }
                }
                else
                {
                    responseMessage = "invoice was already fully paid";
                }

                inv.Save();
            }
            else
            {
                // We can also check here if we need to reverse the payment and extend this case to reverse the payment.
                responseMessage = "no payment needed";
            }

            return responseMessage;
        }
    }
}