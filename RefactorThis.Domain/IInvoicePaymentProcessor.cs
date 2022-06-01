using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public interface IInvoicePaymentProcessor
	{
		string ProcessPayment(Payment payment);
	}
}