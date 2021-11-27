using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefactorThis.Domain.Rule.Interfaces;
using RefactorThis.Persistence.Model;
using String = System.String;

namespace RefactorThis.Domain.Rule.Service
{
    public class InvoiceNullValidator : IPaymentRule
    {
        public (Invoice, string, bool) Process(Invoice invoice, Payment payment)
        {
            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }
            return (invoice, String.Empty ,true);
        }
        public bool IsTerminate()
        {
            return false;
        }
    }
}