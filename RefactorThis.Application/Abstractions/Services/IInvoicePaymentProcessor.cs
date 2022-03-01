using RefactorThis.Application.Models;

namespace RefactorThis.Application.Abstractions.Services
{
    public interface IInvoicePaymentProcessor
    {
        string ProcessPayment(PaymentDto payment);
    }
}
