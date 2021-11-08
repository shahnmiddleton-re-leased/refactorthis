using RefactorThis.Invoicing.DataAccess.Interfaces;
using RefactorThis.Invoicing.Domain.Model;

namespace RefactorThis.Invoicing.DataAccess.Repository
{
    public class InvoiceWriteRepository : IInvoiceWriteRepository
    {		
		public void SaveInvoice(Invoice invoice)
		{
			//saves the invoice to the database
		}

		public void Add(Invoice invoice)
		{
			FakeSharedInvoice.DodgyInvoice = invoice;
		}		
	}
}
