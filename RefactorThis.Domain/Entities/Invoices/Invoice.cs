using System.Collections.Generic;

namespace RefactorThis.Domain.Entities.Invoices
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();

        public void AddPayment(Payment payment)
        {
            this.AmountPaid += payment.Amount;
            this.Payments.Add(payment);
        }
    }
}
