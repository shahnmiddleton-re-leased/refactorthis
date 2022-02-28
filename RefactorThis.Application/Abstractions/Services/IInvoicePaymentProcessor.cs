using RefactorThis.Domain.Entities.Invoice;

namespace RefactorThis.Application.Abstractions.Services
{
    public interface IInvoicePaymentProcessor
    {
        string ProcessPayment(Payment payment);
    }
}
