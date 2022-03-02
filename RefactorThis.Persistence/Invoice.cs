using System.Collections.Generic;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;
		public Invoice( InvoiceRepository repository )
		{
			_repository = repository;
			Payments = new List<Payment>();
		}

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }
	}
}