using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Domain.Entities
{
    public class Invoice
    {
        readonly List<Payment> _payments = new List<Payment>();
        public Invoice()
        { }

        public Invoice(string id, IEnumerable<Payment> payments)
        {
            Id = id;
            _payments = payments.ToList();
        }

        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal AmountPaid => Payments.Sum(i => i.Amount);
        public decimal AmountDue => Amount - Payments.Sum(i => i.Amount);
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
        public bool IsFullyPaid => AmountPaid == Amount;

    }
}
