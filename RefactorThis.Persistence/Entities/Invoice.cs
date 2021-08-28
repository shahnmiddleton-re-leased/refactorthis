using System.Collections.Generic;

namespace RefactorThis.Persistence.Entities
{
	public class Invoice
	{
        public Invoice()
		{
            Payments = new List<Payment>();
        }

		// public void Save( )
		// {
		// 	_repository.SaveInvoice( this );
		// }
  //
  //       public string AddPayment(Payment payment)
  //       {
  //           PaymentResult result = this._paymentValidator.Validate(this, payment);
  //
  //           if (result.AddPayment)
  //           {
		// 		this.Payments.Add(payment);
  //           }
  //
  //           return result.ResponseMessage;
		// }

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }
	}
}