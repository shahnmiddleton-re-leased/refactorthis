using System.Collections;
using System.Collections.Generic;

namespace RefactorThis.Persistence {
	
	public class InvoiceRepository
	{
		// TODO load the _invoiceMap from DB.
		private readonly Dictionary<string,Invoice> _invoiceMap;

		private static InvoiceRepository _invoiceRepository;

		
		public static InvoiceRepository GetInvoiceRepository()
		{
			if (_invoiceRepository == null)
			{
				_invoiceRepository = new InvoiceRepository();
			}

			return _invoiceRepository;
		}

		private InvoiceRepository()
		{
			_invoiceMap = new Dictionary<string, Invoice>();
		}

		public Invoice GetInvoice( string reference )
		{
			if (reference != null && _invoiceMap.ContainsKey(reference))
			{
				return _invoiceMap[reference];;
			}
			
			return null;
		}

		public void SaveInvoice( Invoice invoice )
		{
			//saves the invoice to the database
		}

		public void Add( string reference, Invoice invoice )
		{
			_invoiceMap.Add(reference, invoice);
		}
	}
}