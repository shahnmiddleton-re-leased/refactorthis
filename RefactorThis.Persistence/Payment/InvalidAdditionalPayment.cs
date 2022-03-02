namespace RefactorThis.Persistence.Payment
{
    public class InvalidAdditionalPayment : IPayment
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }

        public string ProcessPayment(decimal amount)
        {
            return "the payment is greater than the partial amount remaining";            
        }
    }
}
