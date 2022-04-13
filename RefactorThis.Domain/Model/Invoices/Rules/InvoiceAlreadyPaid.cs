namespace RefactorThis.Domain.Model.Invoices.Rules
{
    public class InvoiceAlreadyPaid : IPaymentRule
    {
        public bool MatchesFor(Invoice invoice, Payment payment)
        {
            return invoice.PaymentsRunningTotal != 0 && invoice.Amount == invoice.PaymentsRunningTotal;
        }

        public string ValidationMessage => "invoice was already fully paid";
    }
}