using RefactorThis.Persistence;
using RefactorThis.Persistence.Payment;
using System;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(IPayment payment)
        {
            var invoice = _invoiceRepository.GetInvoice(payment.Reference);

            ValidateInput(invoice);

            string responseMessage = "no payment needed";

            if (invoice.Amount != 0)
            {
                IPayment paymentType = PaymentFactory.GetPaymentType(invoice, payment.Amount);
                responseMessage = paymentType.ProcessPayment(payment.Amount);
                invoice.Save();
            }

            return responseMessage;
        }

        private void ValidateInput(Invoice invoice)
        {
            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }
        }
    }
}