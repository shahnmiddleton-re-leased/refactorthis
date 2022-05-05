using System.Threading.Tasks;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.InvoicePaymentProcessorService
{
    public interface IInvoicePaymentProcessorService
    {
        public Task<string> ProcessPaymentAsync(Payment payment);
    }
}