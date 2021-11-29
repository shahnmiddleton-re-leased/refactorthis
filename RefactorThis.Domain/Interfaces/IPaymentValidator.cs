using RefactorThis.Domain.PaymentChecks;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.Interfaces
{
    public interface IPaymentValidator
    {
        PaymentResult Validate( Invoice inv, Payment payment );
    }
}