using System;

namespace Invoicing.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        }

        public string ProcessPayment(Payment payment)
        {
            if (payment is null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            var inv = _invoiceRepository.Get(payment.Reference);

            string responseMessage;
            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }
            else
            {
                responseMessage = inv.AddPayment(payment);
            }

            inv.Save();

            return responseMessage;
        }
    }
}