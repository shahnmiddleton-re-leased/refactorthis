namespace RefactorThis.Domain.Model.Invoices.Rules
{
    public class PaymentGreaterThanInvoiceAmount : IPaymentRule
    {
        public bool MatchesFor(Invoice invoice, Payment payment)
        {
            return payment.Amount > invoice.Amount;
        }

        public string ValidationMessage => "the payment is greater than the invoice amount";
    }
}