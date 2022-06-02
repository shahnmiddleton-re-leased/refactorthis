namespace RefactorThis.Domain.Model
{
	public class Payment
	{
		public decimal Amount { get; set; }

		public string Reference { get; set; }

		public Payment(string reference)
        {
			Reference = reference;
        }
	}
}