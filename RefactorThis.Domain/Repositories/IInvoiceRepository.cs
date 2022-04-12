using RefactorThis.Domain.Model.Invoices;

namespace RefactorThis.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice(string reference);
        void SaveInvoice(Invoice invoice);
        void Add(Invoice invoice);
    }
}
