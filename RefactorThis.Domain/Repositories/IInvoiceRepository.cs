using RefactorThis.Domain.Model.Invoices;

namespace RefactorThis.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice(string reference);
        void Add(Invoice invoice);
    }
}
