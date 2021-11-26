using RefactorThis.Persistence.Model;

namespace RefactorThis.Persistence {
	public class InvoiceRepository
	{
		private Invoice _invoice;

		public Invoice GetInvoice( string reference )
		{
			return _invoice;
		}

		public void SaveInvoice()
		{
			//saves the invoice to the database
		}

		public void Add( Invoice invoice )
		{
			_invoice = invoice;
		}
	}
}