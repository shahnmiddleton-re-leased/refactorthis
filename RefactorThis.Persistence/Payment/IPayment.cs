namespace RefactorThis.Persistence.Payment
{
	public interface IPayment
	{
		decimal Amount { get; set; }
		string Reference { get; set; }

        string ProcessPayment(decimal amount);
	}
}