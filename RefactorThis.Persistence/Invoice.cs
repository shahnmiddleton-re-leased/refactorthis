using System.Collections.Generic;
using System.ComponentModel;
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
        public decimal AmountPaid => Payments.Sum(payment => payment.Amount);
        public List<Payment> Payments { get; set; }

        public void AddPayment(Payment payment)
        {
            Payments.Add(payment);
        }

        public decimal PaymentPending()
        {
            return (Amount - AmountPaid);
        }
	}
}