using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;
		public Invoice( InvoiceRepository repository )
		{
			_repository = repository;
		}

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get { return HasPayments ? Payments.Sum(x => x.Amount) : 0; } set { /*Only reason to keep setter is for the tests trying to set it*/ } }
		public List<Payment> Payments { get; set; } 
		public bool HasPayments { get { return Payments != null && Payments.Any(); } }

		/// <summary>
		/// Add a payment to the invoice
		/// </summary>
		/// <param name="payment"></param>
		/// <returns>String indicating the result of the payment process</returns>
		public string AddPayment(Payment payment)
        {
			string responseMessage = string.Empty;

			if (Amount == 0)
            {
				if(HasPayments)
                {
					throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
				}
				else
                {
					responseMessage = "no payment needed";
				}
            }
			else if (AmountPaid == Amount)
            {
				responseMessage = "invoice was already fully paid";

			}
			else if (payment.Amount + AmountPaid > Amount)
            {
				responseMessage = HasPayments ? "the payment is greater than the partial amount remaining" : "the payment is greater than the invoice amount";
			}
			else if(payment.Amount + AmountPaid == Amount)
            {
				responseMessage = HasPayments ? "final partial payment received, invoice is now fully paid" : "invoice is now fully paid";
				Payments.Add(payment);
			}
			else 
            {
				responseMessage = HasPayments ? "another partial payment received, still not fully paid" : "invoice is now partially paid";
				Payments.Add(payment);
			}

			return responseMessage;
        }
	}
}