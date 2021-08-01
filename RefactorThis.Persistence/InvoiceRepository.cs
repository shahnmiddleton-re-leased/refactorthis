using System.Threading.Tasks;

namespace RefactorThis.Persistence {
	public class InvoiceRepository
	{
		private Invoice _invoice;

        // NOTE: these methods should be using either an awaited HTTP request, or database query,
        // but for the purpose of this refactor we just return the invoice property

        /// <summary>
        /// Gets an invoice
        /// </summary>
        /// <param name="reference">The reference to obtain the invoice by</param>
        /// <returns>An invoice</returns>
        public async Task<Invoice> GetInvoice(string reference)
        {
			return _invoice;
		}

		/// <summary>
		/// Saves an invoice
		/// </summary>
		/// <param name="invoice">The invoice to save</param>
		/// <returns>The updated invoice</returns>
		public async Task<Invoice> SaveInvoice(Invoice invoice)
		{
			return _invoice;
			//saves the invoice to the database
		}

		/// <summary>
		/// Adds an invoice
		/// </summary>
		/// <param name="invoice">The invoice to add</param>
		public void Add(Invoice invoice)
		{
			_invoice = invoice;
		}
	}
}