using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.Interfaces
{
    public interface IInvoicePaymentProcessor
    {
        string ProcessPayment( Payment payment );
    }
}