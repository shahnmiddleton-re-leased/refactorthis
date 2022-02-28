namespace RefactorThis.Domain.Entities.Invoice
{
    // entity
    public class Payment
    {
        public decimal Amount { get; private set; }
        public string Reference { get; private set; }

        public Payment(decimal amount, string reference)
        {
            Amount = amount;
            Reference = reference;
        }
    }
}
