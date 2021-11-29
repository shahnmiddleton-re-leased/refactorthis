using System.Linq;

namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoiceFinalPartialPayment : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if ((invoice.Amount - invoice.AmountPaid) == payment.Amount && invoice.Payments.Any())
            {
                invoice.AddPayment(payment);
                return "final partial payment received, invoice is now fully paid";
            }

            return null;
        }
    }
}
