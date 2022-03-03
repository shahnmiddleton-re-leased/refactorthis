namespace RefactorThis.Persistence
{
	public class Payment
	{
		public Payment(string reference)
        {
			Reference = reference;
        }

		public decimal Amount { get; set; }
		public string Reference { get; private set; }
	}
}