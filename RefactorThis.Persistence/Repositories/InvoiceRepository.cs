using RefactorThis.Domain.Entities.Invoices;
using RefactorThis.Domain.Interfaces;

namespace RefactorThis.Persistence
{
    public class InvoiceRepository : IInvoiceRepository
    {
        Invoice _invoice;

        public Invoice FindInvoiceByReference(string reference)
        {
            // Pretend to find something based on the reference.
            return _invoice;
        }

        public void Update(Invoice invoice)
        {
            _invoice = invoice;
        }
    }
}