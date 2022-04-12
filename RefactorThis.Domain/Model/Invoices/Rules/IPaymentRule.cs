namespace RefactorThis.Domain.Model.Invoices.Rules
{
    public interface IPaymentRule
    {
        bool MatchesFor(Invoice invoice, Payment payment);
        string ValidationMessage { get; }
    }
}