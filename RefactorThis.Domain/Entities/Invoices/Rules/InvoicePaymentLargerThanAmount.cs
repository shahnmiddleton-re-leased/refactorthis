using System.Linq;

namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoicePaymentLargerThanAmount : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if (invoice.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid))
                return "the payment is greater than the partial amount remaining";

            return null;
        }
    }
}
