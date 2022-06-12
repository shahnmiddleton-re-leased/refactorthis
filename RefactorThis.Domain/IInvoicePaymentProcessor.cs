using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    public interface IInvoicePaymentProcessor
    {
        string ProcessPayment(Payment payment);
    }
}
