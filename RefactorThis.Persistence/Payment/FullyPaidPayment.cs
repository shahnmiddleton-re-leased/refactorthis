namespace RefactorThis.Persistence.Payment
{
    public class FullyPaidPayment : IPayment
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }

        public string ProcessPayment(decimal amount)
        {
            return "invoice was already fully paid";            
        }
    }
}
