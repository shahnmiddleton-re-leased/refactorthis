using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.Interfaces
{
    public interface IInvoiceService
    {
        void SaveInvoice(Invoice invoice);
        Invoice GetInvoice(string reference);
        string AddPaymentToInvoice(Invoice invoice, Payment payment);
    }
}