using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		public decimal Amount { get; set; }
		public IList<Payment> Payments { get; set; }
		public decimal AmountPaid => Payments.Sum(p => p.Amount);
		public decimal RemainingBalance => Amount - AmountPaid;

	}
}