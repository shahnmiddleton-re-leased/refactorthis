using System.Collections.Generic;

namespace RefactorThis.Domain.Model
{
	public class Invoice : BaseModel
	{
        public Invoice(string id) : base(id)
        {
        }

        public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }
	}
}