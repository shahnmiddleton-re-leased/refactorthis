
namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    internal class InvoiceFullyPaid : IPaymentRule
    {
        public string ProcessPaymentRule(Invoice invoice, Payment payment)
        {
            if (invoice.Amount == payment.Amount)
            {
                invoice.AddPayment(payment);
                return "invoice is now fully paid";
            }

            return null;
        }
    }
}
