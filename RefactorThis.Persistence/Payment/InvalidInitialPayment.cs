namespace RefactorThis.Persistence.Payment
{
    public class InvalidInitialPayment : IPayment
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }

        public string ProcessPayment(decimal amount)
        {
            return "the payment is greater than the invoice amount";
        }
    }
}
