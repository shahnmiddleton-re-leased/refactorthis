using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly IInvoiceRepository _repository;
        private readonly IList<Payment> _payments;

		public Invoice(IInvoiceRepository repository)
		{
			_repository = repository;
            _payments = new List<Payment>();
        }

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal Amount { get; set; }
        public decimal AmountPaid => _payments.Sum(payment => payment.Amount);

        public void AddPayment(Payment payment)
        {
            _payments.Add(payment);
        }

        public decimal PaymentPending()
        {
            return (Amount - AmountPaid);
        }
	}
}