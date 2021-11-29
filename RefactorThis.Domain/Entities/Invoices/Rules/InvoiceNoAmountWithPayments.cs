using System.Linq;

namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoiceNoAmountWithPayments : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if (invoice.Amount == 0 && invoice.Payments.Any())
                return "The invoice is in an invalid state, it has an amount of 0 and it has payments.";

            return null;
        }
    }
}
