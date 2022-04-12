using RefactorThis.Domain.Model.Invoices;
using RefactorThis.Domain.Repositories;

namespace RefactorThis.Persistence {
    public class InvoiceRepository : IInvoiceRepository
    {
		public Invoice GetInvoice( string reference )
		{
			return null;
		}

		public void Add( Invoice invoice )
		{
		}
	}
}