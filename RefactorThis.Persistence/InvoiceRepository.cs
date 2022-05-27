using System;
using System.Collections.Generic;

namespace RefactorThis.Persistence
{
    public class InvoiceRepository
    {
        private readonly Dictionary<string, Invoice> _invoices = new Dictionary<string, Invoice>();

        public Invoice GetInvoice(string reference)
        {
            return _invoices.ContainsKey(reference) ? _invoices[reference] : null;
        }

        public void Add(Invoice invoice)
        {
            _invoices.Add(invoice.Reference, invoice);
        }
    }
}