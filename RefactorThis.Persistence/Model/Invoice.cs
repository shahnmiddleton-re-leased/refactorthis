using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Model
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
