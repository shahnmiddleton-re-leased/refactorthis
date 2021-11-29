
using System.Linq;

namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoiceNoPaymentNeeded : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if (invoice.Amount == 0 && !invoice.Payments.Any())
                return "no payment needed";

            return null;
        }
    }
}
