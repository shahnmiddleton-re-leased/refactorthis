using RefactorThis.Persistence.Model;

namespace RefactorThis.Domain.Rule.Interfaces
{
    public interface IPaymentRule
    {
        (Invoice, string, bool) Process(Invoice invoice, Payment payment);
        bool IsTerminate();
    }
}
