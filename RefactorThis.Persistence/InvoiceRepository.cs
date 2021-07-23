using System;
using System.Collections.Generic;

namespace RefactorThis.Persistence 
{
    /// <summary>
    /// An in-memory collection of <see cref="Invoice"/>s
    /// </summary>
    public class InvoiceRepository
    { 
        private readonly Dictionary<string, Invoice> _invoices;

        /// <summary>
        /// Gets the invoice by invoice number.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <returns>The invoice if found, otherwise <c>null</c></returns>
        public Invoice GetInvoiceByInvoiceNumber(string invoiceNumber)
        {
            return _invoices.ContainsKey(invoiceNumber) ? _invoices[invoiceNumber] : null;
        }

        /// <summary>
        /// Saves the invoice to the database.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <returns><c>true</c> if saved, <c>false</c> otherwise.</returns>
        public bool SaveInvoice(Invoice invoice)
		{
            try
            {
                if (_invoices.ContainsKey(invoice.InvoiceNumber))
                {
                    _invoices[invoice.InvoiceNumber] = invoice;

                    // TODO: UPDATE Invoice in database
                }
                else
                {
                    _invoices.Add(invoice.InvoiceNumber, invoice);

                    // TODO: INSERT Invoice into database
                }
                
                return true;
            }
            catch (Exception ex)
            {
                // TODO: Log this appropriately
                Console.WriteLine($"Failed to save new invoice to database - {ex.Message}");

                return false;
            }
        }

        private static IEnumerable<Invoice> LoadExistingInvoices()
        {
			// TODO: SELECT these from the database
            return new List<Invoice>();
        }

        #region Singleton Instance

        private static readonly object _singletonInstanceLock = new object();
        private static InvoiceRepository _instance;

        public static InvoiceRepository Instance
        {
            get
            {
                lock (_singletonInstanceLock)
                {
                    if (_instance is null)
                    {
                        _instance = new InvoiceRepository();
                    }
                }

                return _instance;
            }
        }

        private InvoiceRepository()
        {
            _invoices = new Dictionary<string, Invoice>();

            var databaseInvoices = LoadExistingInvoices();

            foreach (var databaseInvoice in databaseInvoices)
            {
                _invoices.Add(databaseInvoice.InvoiceNumber, databaseInvoice);
            }
        }

        #endregion
    }
}