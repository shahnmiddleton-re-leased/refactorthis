
using RefactorThis.Invoicing.Domain.Model;

namespace RefactorThis.Invoicing.DataAccess.Interfaces
{
    public interface IInvoiceReadRepository
    {
        Invoice GetInvoice(string reference);        
    }
}
