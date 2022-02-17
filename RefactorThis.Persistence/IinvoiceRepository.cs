using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence
{
    public interface IinvoiceRepository
    {
        Invoice GetInvoice(string reference);
        void SaveInvoice(Invoice invoice);
        void Add(Invoice invoice);
    }
}
