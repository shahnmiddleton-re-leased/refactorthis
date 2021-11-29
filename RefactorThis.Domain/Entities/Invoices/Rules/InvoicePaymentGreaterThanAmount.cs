
namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoicePaymentGreaterThanAmount : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if (payment.Amount > invoice.Amount)
                return "the payment is greater than the invoice amount";

            return null;
        }
    }
}
