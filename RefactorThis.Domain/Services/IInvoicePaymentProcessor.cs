using RefactorThis.Domain.Model;

namespace RefactorThis.Domain.Services
{
    public interface IInvoicePaymentProcessor
    {
        string ProcessPayment(Payment payment);
    }
}
