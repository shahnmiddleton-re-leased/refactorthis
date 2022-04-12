namespace RefactorThis.Domain.Model
{
	public class Payment
	{
        public Payment(decimal amount, string reference)
        {
            Amount = amount;
            Reference = reference;
        }

        private Payment(){}

        public decimal Amount { get; private set; }
		public string Reference { get; private set; }
	}
}