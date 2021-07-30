using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public interface IInvoicePaymentProcessorService
    {
        public string ProcessPayment(Payment payment);
    }
}