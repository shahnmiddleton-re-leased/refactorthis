
namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoicePartiallyPaid : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if (payment.Amount < invoice.Amount)
            {
                invoice.AddPayment(payment);
                return "invoice is now partially paid";
            }

            return null;
        }
    }
}
