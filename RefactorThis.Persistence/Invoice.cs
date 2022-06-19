using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
    public class Invoice
    {
        private readonly InvoiceRepository _repository;
        public decimal Amount { get; set; }
        public List<Payment> Payments { get; set; }

        public Invoice(InvoiceRepository repository)
        {
            _repository = repository;
        }

        public decimal AmountPaid()
        {
            return Payments.Sum(x => x.Amount);
        }

        public string ProcessPayment(Payment payment)
        {
            var amoundPaid = AmountPaid();
            Payments.Add(payment);
            _repository.SaveInvoice(this);

            if (amoundPaid > 0)
            {
                if ((Amount - amoundPaid) == payment.Amount)
                {
                    return "final partial payment received, invoice is now fully paid";
                }
                return "another partial payment received, still not fully paid";
            }

            if (Amount == payment.Amount)
            {
                return "invoice is now fully paid";
            }
            return "invoice is now partially paid";
        }
    }
}