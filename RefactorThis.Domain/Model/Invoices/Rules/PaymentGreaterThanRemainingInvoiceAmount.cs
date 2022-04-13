namespace RefactorThis.Domain.Model.Invoices.Rules
{
    public class PaymentGreaterThanRemainingInvoiceAmount : IPaymentRule
    {
        public bool MatchesFor(Invoice invoice, Payment payment)
        {
            return invoice.PaymentsRunningTotal != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid);
        }

        public string ValidationMessage => "the payment is greater than the partial amount remaining";
    }
}