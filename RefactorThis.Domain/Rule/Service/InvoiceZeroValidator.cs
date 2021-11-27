using System;
using System.Linq;
using RefactorThis.Domain.Rule.Interfaces;
using RefactorThis.Persistence.Model;
using String = System.String;

namespace RefactorThis.Domain.Rule.Service
{
    public class InvoiceZeroValidator : IPaymentRule
    {
        private bool _terminate;

        public InvoiceZeroValidator()
        {
            _terminate = false;
        }
        public (Invoice, string) Process(Invoice invoice, Payment payment)
        {
            string responseMessage = string.Empty;
            if (invoice.Amount == 0)
            {
                if (invoice.Payments == null || !invoice.Payments.Any())
                {
                    responseMessage = "no payment needed";
                    _terminate = true;
                }
                else
                {
                    throw new InvalidOperationException(
                        "The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }
            return (invoice, responseMessage);
        }
        public bool IsTerminate()
        {
            return _terminate;
        }
    }
}
