namespace RefactorThis.Persistence
{
	public class Payment
	{
		/// <summary>
		/// The amount on the payment
		/// </summary>
		public decimal Amount { get; set; }
		/// <summary>
		/// The invoice reference for the payment
		/// </summary>
		public string Reference { get; set; }
	}
}