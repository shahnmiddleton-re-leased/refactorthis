using System.Collections.Generic;

namespace RefactorThis.Persistence.Model
{
	public class Invoice
	{
		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }

		public void AddPayment(Payment payment)
		{
			this.AmountPaid += payment.Amount;
			if (this.Payments == null)
			{
				Payments = new List<Payment>();
			}
			this.Payments.Add(payment);
		}
	}
}