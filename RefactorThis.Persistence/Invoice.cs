using RefactorThis.Persistence.Payment;
using System.Collections.Generic;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly IInvoiceRepository _repository;
		public Invoice(IInvoiceRepository repository )
		{
			_repository = repository;
		}

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<IPayment> Payments { get; set; }
	}
}