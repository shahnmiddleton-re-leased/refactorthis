using RefactorThis.Application.Common.Interfaces;
using RefactorThis.Domain.Entities;
using System.Threading.Tasks;

namespace RefactorThis.Persistence
{
    public class InvoiceRepository : IInvoiceRepository
    {
        public Task<Invoice> GetByIdAsync(string id)
        {
            // if not found it should throw exception
            // throw new NotFoundException(nameof(Invoice), Id); 

            return Task.FromResult(new Invoice());
        }
    }
}