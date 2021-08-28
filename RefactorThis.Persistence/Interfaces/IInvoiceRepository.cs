using RefactorThis.Persistence.Entities;

namespace RefactorThis.Persistence.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice( string reference );
        void SaveInvoice( Invoice invoice );
        void Add( Invoice invoice );
    }
}