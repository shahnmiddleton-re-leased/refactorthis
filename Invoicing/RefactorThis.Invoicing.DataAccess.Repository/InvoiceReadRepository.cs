using RefactorThis.Invoicing.DataAccess.Interfaces;
using RefactorThis.Invoicing.Domain.Model;

namespace RefactorThis.Invoicing.DataAccess.Repository
{
    public class InvoiceReadRepository : IInvoiceReadRepository
    {
        public Invoice GetInvoice(string reference)
        {
            return FakeSharedInvoice.DodgyInvoice;
        }
    }
}
