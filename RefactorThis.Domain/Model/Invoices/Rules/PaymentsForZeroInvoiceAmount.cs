namespace RefactorThis.Domain.Model.Invoices.Rules
{
    public class PaymentsForZeroInvoiceAmount : IPaymentRule
    {
        public bool MatchesFor(Invoice invoice, Payment payment)
        {
            return invoice.Amount == 0 && invoice.HasPayments;
        }

        public string ValidationMessage =>
            "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
    }
}