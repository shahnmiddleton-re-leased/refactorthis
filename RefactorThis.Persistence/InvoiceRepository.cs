using RefactorThis.Domain.Abstractions.Repositories;
using RefactorThis.Domain.Entities.Invoice;

namespace RefactorThis.Persistence
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private Invoice _invoice;

#pragma warning disable IDE0060 // Remove unused parameter
        public Invoice GetInvoice(string reference)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return _invoice;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public void SaveInvoice(Invoice invoice)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //saves the invoice to the database
        }

        public void Add(Invoice invoice)
        {
            _invoice = invoice;
        }
    }
}
