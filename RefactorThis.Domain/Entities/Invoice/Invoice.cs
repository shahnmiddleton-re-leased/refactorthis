using System.Collections.Generic;

namespace RefactorThis.Domain.Entities.Invoice
{
    public partial class Invoice
    {
        public decimal Amount { get; private set; }
        public decimal AmountPaid { get; private set; }
        public List<Payment> Payments { get; private set; }

        public Invoice()
        {
            Payments = new List<Payment>();
        }
    }
}
