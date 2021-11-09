using RefactorThis.Common.Interfaces.Rules;
using RefactorThis.Invoicing.Domain.Model;

namespace RefactorThis.Invoicing.Domain.Interfaces
{
    public interface IInvoicePaymentProcessor
    {
        IRuleContext ProcessPayment(Payment payment);
        void AddInvoice(Invoice invoice);
    }
}
