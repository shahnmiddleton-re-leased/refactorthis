namespace RefactorThis.Persistence {
	/// <summary>
	/// Class representation of an Invoice Repository
	/// </summary>
	public class InvoiceRepository
	{
        #region Declarations
        private Invoice _invoice;
        #endregion

        #region Methods
        /// <summary>
        /// Retrieves the Invoice data by payment reference
        /// </summary>
        /// <param name="reference"></param>
        /// <returns>Invoice</returns>
        public Invoice GetInvoice( string reference )
		{
			return _invoice;
		}

		/// <summary>
		/// Saves the Invoice data to the database.
		/// </summary>
		/// <param name="invoice"></param>
		public void SaveInvoice( Invoice invoice )
		{
			//saves the invoice to the database
		}

		/// <summary>
		/// Sets/adds a new Invoice
		/// </summary>
		/// <param name="invoice"></param>
		public void Add( Invoice invoice )
		{
			_invoice = invoice;
		}
        #endregion 
    }
}