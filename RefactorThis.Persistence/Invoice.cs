using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;
		private Invoice( InvoiceRepository repository )
		{
			_repository = repository;
		}

		private void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public static Invoice Create( InvoiceRepository repository, decimal amount, decimal amountPaid, List<Payment> payments)
        {
			var invoice = new Invoice( repository )
			{
				Amount = amount,
				AmountPaid = amountPaid,
				Payments = payments
			};

			return invoice;
		}

		public string AddPayment( Payment payment )
		{
			string responseMessage;

			if ( Amount == 0 )
			{
				if ( Payments == null || !Payments.Any( ) )
				{
					return "no payment needed";
				}
				else
				{
					throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
				}
			}

			if ( Payments != null && Payments.Any( ) )
			{
				responseMessage = HandleConcecutivePayments(payment);
			}
			else
			{
				responseMessage = HandleFirstPayment(payment);
			}

			return responseMessage;
		}

		private string HandleConcecutivePayments(Payment payment)
        {
			var sum = Payments.Sum(x => x.Amount);

			if (sum != 0 && Amount == sum)
			{
				return "invoice was already fully paid";
			}

			if (sum != 0 && payment.Amount > (Amount - AmountPaid))
			{
				return "the payment is greater than the partial amount remaining";
			}

			var responseMessage = (Amount - AmountPaid) == payment.Amount ? "final partial payment received, invoice is now fully paid" :
				"another partial payment received, still not fully paid";

			AmountPaid += payment.Amount;
			Payments.Add(payment);

			Save();

			return responseMessage;
		}

		private string HandleFirstPayment(Payment payment)
        {
			if (payment.Amount > Amount)
			{
				return "the payment is greater than the invoice amount";
			}

			if (Payments == null)
				Payments = new List<Payment>();

			var responseMessage = Amount == payment.Amount ? "invoice is now fully paid" : "invoice is now partially paid";

			AmountPaid = payment.Amount;
			Payments.Add(payment);

			Save();

			return responseMessage;
		}

		private decimal Amount { get; set; }
		private decimal AmountPaid { get; set; }
		private List<Payment> Payments { get; set; }
	}
}