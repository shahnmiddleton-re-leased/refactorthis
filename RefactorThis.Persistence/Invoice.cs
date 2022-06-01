using System;
using System.Collections.Generic;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }
	}
}