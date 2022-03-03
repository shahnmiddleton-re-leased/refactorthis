using Microsoft.Extensions.Logging;
using System;

namespace RefactorThis.Persistence {
	public class InvoiceRepository : IInvoiceRepository 
	{
		private Invoice _invoice;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository()
        {
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

		public Invoice GetInvoice(string reference)
		{
			//_logger.LogInformation("Attempting to get invoice for reference ID: {ReferenceID}", reference);
			return _invoice;
		}

		public void SaveInvoice( Invoice invoice )
		{
			//saves the invoice to the database
			//_logger.LogInformation("Saving invoice {}");
		}

		public void Add( Invoice invoice )
		{
			_invoice = invoice;
		}
	}
}