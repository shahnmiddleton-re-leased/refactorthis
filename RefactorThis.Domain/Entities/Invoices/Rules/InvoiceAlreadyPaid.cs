using System.Linq;

namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoiceAlreadyPaid : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if (invoice.Amount == invoice.Payments.Sum(x => x.Amount))
                return "invoice was already fully paid";

            return null;
        }
    }
}
