namespace RefactorThis.Domain.Model.Invoices.Rules
{
    public class NoPaymentNeeded : IPaymentRule
    {
        public bool MatchesFor(Invoice invoice, Payment payment)
        {
            return invoice.Amount == 0 && !invoice.HasPayments;
        }

        public string ValidationMessage => "no payment needed";
    }
}