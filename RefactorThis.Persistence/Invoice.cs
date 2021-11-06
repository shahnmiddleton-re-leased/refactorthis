using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		/// <summary>
		/// For unit testing there are a variety of ways we could access the private setters:
		/// - make them internal, move Invoice to an assembly and use InternalsVisibleToAttribute for the unit tests.
		///   (a reasonable solution in a real production environment, although assembly scope is req'd for internal).
		/// - use Reflection in the unit tests to find the private setters (a little ugly).
		/// - do this: provide a simple helper function that's used by the unit tests (also a little ugly).
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="amountPaid"></param>
		public static Invoice CreateTestInvoice(InvoiceRepository repo, decimal amount = 0, decimal amountPaid = 0, 
			List<Payment> payments = null)
		{
			var testInvoice = new Invoice(repo);
			testInvoice.Amount = amount;
			testInvoice.AmountPaid = amountPaid;
			testInvoice.Payments = payments;
			return testInvoice;
		}

		private readonly InvoiceRepository _repository;
		public Invoice( InvoiceRepository repository )
		{
			_repository = repository;
		}

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		/// <summary>
		/// It's dangerous to expose writeable state to the world, particularly if it's an invoice!
		/// </summary>
		public decimal Amount { get; private set; }
		public decimal AmountPaid { get; private set; }
		public decimal AmountOutstanding { get { return Amount - AmountPaid; } }
		private List<Payment> Payments { get; set; }

		/// <summary>
		/// Does this invoice have payments?
		/// </summary>
		/// <param name="invoice"></param>
		/// <returns></returns>
		public bool HasPayments()
		{
			return Payments != null && Payments.Any();
		}

		/// <summary>
		/// Returns the total value of payments made against the invoice.
		/// </summary>
		/// <param name="invoice"></param>
		/// <returns></returns>
		public decimal TotalPayments()
		{
			return Payments.Sum(x => x.Amount);
		}

		/// <summary>
		/// Make a payment to this invoice, updating it appropriately and saving the new invoice state.
		/// </summary>
		public void MakePayment(Payment payment)
		{
			// TODO: In a real product you'd add some form of audit trail here most definitely as the invoice
			// state is changing.
			AmountPaid += payment.Amount;
			Payments.Add(payment);
			Save();
		}
	}
}