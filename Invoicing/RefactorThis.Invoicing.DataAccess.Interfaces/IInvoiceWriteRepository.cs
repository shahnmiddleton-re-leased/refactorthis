using RefactorThis.Invoicing.Domain.Model;

namespace RefactorThis.Invoicing.DataAccess.Interfaces
{
    public interface IInvoiceWriteRepository
    {
        void SaveInvoice(Invoice invoice);
        void Add(Invoice invoice);
    }
}
