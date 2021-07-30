using System.Threading.Tasks;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public interface IInvoicePaymentProcessorService
    {
        public Task<string> ProcessPaymentAsync(Payment payment);
    }
}