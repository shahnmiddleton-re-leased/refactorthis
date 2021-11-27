using System;
using System.Linq;
using RefactorThis.Domain.Rule.Interfaces;
using RefactorThis.Persistence.Model;
using String = System.String;

namespace RefactorThis.Domain.Rule.Service
{
    public class InvoiceExistingPaymentsProcess : IPaymentRule
    {
        private bool _terminate;

        public InvoiceExistingPaymentsProcess()
        {
            _terminate = false;
        }

        private bool ExistingPaymentIsFullyPaid(Invoice invoice, Payment payment)
        {
            return invoice.Payments.Sum(x => x.Amount) != 0 && invoice.Amount == invoice.Payments.Sum(x => x.Amount);
        }
        private bool IsGreaterThanRemaining(Invoice invoice, Payment payment)
        {
            return invoice.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid);
        }
        private bool IsFullyPaid(Invoice invoice, Payment payment)
        {
            return (invoice.Amount - invoice.AmountPaid) == payment.Amount;
        }
        public (Invoice, string, bool) Process(Invoice invoice, Payment payment)
        {
            string responseMessage = string.Empty;
            bool success = false;
            // We code broke it to multiple rules 
            if (invoice.Payments != null && invoice.Payments.Any())
            {
                if (ExistingPaymentIsFullyPaid(invoice,payment))
                {
                    responseMessage = "invoice was already fully paid";
                    _terminate = true;
                    success = false;
                }
                else if (IsGreaterThanRemaining(invoice, payment))
                {
                    responseMessage = "the payment is greater than the partial amount remaining";
                    _terminate = true;
                    success = false;

                }
                else
                {
                    success = true;
                    if (IsFullyPaid(invoice, payment))
                    {
                        invoice.AmountPaid += payment.Amount;
                        invoice.Payments.Add(payment);
                        responseMessage = "final partial payment received, invoice is now fully paid";
                        _terminate = true;

                    }
                    else
                    {
                        invoice.AmountPaid += payment.Amount;
                        invoice.Payments.Add(payment);
                        responseMessage = "another partial payment received, still not fully paid";
                        _terminate = true;
                    }
                }
            }
            return (invoice, responseMessage, success);
        }
        public bool IsTerminate()
        {
            return _terminate;
        }
    }
}