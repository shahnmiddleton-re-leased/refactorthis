using System.Collections.Generic;

namespace RefactorThis.Persistence.Entities
{
	public class Invoice
	{
        public Invoice( )
		{
            Payments = new List<Payment>( );
        }

        public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }
	}
}