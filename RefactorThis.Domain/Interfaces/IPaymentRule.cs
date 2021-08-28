using RefactorThis.Domain.PaymentChecks;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.Interfaces
{
    public interface IPaymentRule
    {
        PaymentResult RunRule(Invoice invoice, Payment payment);
    }
}
