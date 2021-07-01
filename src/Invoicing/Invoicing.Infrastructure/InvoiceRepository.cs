using Invoicing.Domain;
using System;

namespace Invoicing.Infrastructure
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private Invoice _invoice;

        public Invoice Get(string reference)
        {
            // TODO: add null checking here
            return _invoice;
        }

        public void Update(Invoice invoice)
        {
            if (invoice is null)
            {
                throw new ArgumentNullException(nameof(invoice));
            }
            //saves the invoice to the database
        }

        public void Add(Invoice invoice)
        {
            _invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
        }
    }
}