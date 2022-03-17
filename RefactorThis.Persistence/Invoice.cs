using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
    public class Invoice
    {
        private readonly InvoiceRepository _repository;

        public Invoice(InvoiceRepository repository)
        {
            _repository = repository;
        }

        public void Save()
        {
            _repository.SaveInvoice(this);
        }

        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }

        // Add helper property for processing
        public decimal AmountOutstanding => Amount - Payments.Sum(p => p.Amount);

        public List<Payment> Payments { get; set; } = new List<Payment>(); // Avoid null checking
    }
}