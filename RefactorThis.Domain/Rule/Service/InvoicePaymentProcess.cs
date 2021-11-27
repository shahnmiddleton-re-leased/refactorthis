using System;
using System.Linq;
using RefactorThis.Domain.Rule.Interfaces;
using RefactorThis.Persistence.Model;
using String = System.String;

namespace RefactorThis.Domain.Rule.Service
{
    public class InvoicePaymentProcess : IPaymentRule
    {
        private bool _terminate;

        public InvoicePaymentProcess()
        {
            _terminate = false;
        }
        public (Invoice, string, bool) Process(Invoice invoice, Payment payment)
        {
            string responseMessage = string.Empty;
            bool success = false;

            if (payment.Amount > invoice.Amount)
            {
                responseMessage = "the payment is greater than the invoice amount";
                _terminate = true;
                success = false;
            }
            else if (invoice.Amount == payment.Amount)
            {
                invoice.AmountPaid = payment.Amount;
                invoice.Payments.Add(payment);
                responseMessage = "invoice is now fully paid";
                _terminate = true;
                success = true;

            }
            else
            {
                invoice.AmountPaid = payment.Amount;
                invoice.Payments.Add(payment);
                responseMessage = "invoice is now partially paid";
                _terminate = true;
                success = true;
            }
            return (invoice, responseMessage, success);
        }
        public bool IsTerminate()
        {
            return _terminate;
        }
    }
}
