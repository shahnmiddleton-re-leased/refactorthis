using System.Collections.Generic;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		
        public String InvoiceReference { get; set; }
        public decimal AmountDue { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }
	}
}
