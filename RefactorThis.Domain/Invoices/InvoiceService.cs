using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;
using RefactorThis.Persistence.Interfaces;

namespace RefactorThis.Domain.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentValidator _paymentValidator;

        public InvoiceService(IInvoiceRepository invoiceRepository, IPaymentValidator paymentValidator)
        {
            _invoiceRepository = invoiceRepository;
            _paymentValidator = paymentValidator;
        }
        public void SaveInvoice(Invoice invoice)
        {
            _invoiceRepository.SaveInvoice(invoice);
        }

        public Invoice GetInvoice(string reference)
        {
            return _invoiceRepository.GetInvoice(reference);
        }

        public string AddPaymentToInvoice(Invoice inv, Payment payment)
        {
            var result = this._paymentValidator.Validate(inv, payment);

            if (result.AddPayment)
            {
                inv.Payments.Add(payment);
            }

            return result.ResponseMessage;
        }
    }
}
