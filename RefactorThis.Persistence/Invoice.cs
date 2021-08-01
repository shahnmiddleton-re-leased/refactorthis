using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;

		/// <summary>
		/// Creates a new invoice with provided repository
		/// </summary>
		/// <param name="repository">The repository to create the invoice with</param>
		public Invoice(InvoiceRepository repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// The amount owed on the invoice
		/// </summary>
		public decimal Amount { get; set; }
		/// <summary>
		/// The amount already paid on the invoice
		/// </summary>
		public decimal AmountPaid { get; set; }

		/// <summary>
		/// The payments made on the invoice
		/// </summary>
		public List<Payment> _payments;
		public List<Payment> Payments
        {
			get 
			{
				if (_payments == null)
					_payments = new List<Payment>();

				return _payments;
			}
			set
            {
				_payments = value;
            }
		}

		/// <summary>
		/// Saves an invoice
		/// </summary>
		/// <param name="invoice">The invoice to save</param>
		/// <returns>The saved invoice</returns>
		public async Task Save(Invoice invoice)
		{
			await _repository.SaveInvoice(this);
		}
	}
}