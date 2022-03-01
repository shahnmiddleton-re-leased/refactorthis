namespace RefactorThis.Persistence {
	public class InvoiceRepository: IInventoryRepository
	{
		private Invoice _invoice;

		public Invoice GetInvoice(string refer)
		{
			return _invoice;
		}

		public void SaveInvoice( Invoice invoice )
		{
			//saves the invoice to the database
		}

		public void Add( Invoice invoice )
		{
			_invoice = invoice;
		}
	}
}