using System.Linq;

namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoicePartialPaymentIncomplete : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if ((invoice.Amount - invoice.AmountPaid) > payment.Amount && invoice.Payments.Any())
            {
                invoice.AddPayment(payment);
                return "another partial payment received, still not fully paid";
            }

            return null;
        }
    }
}
