using System;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor() : this(new InvoiceRepository())
        {
        }

        public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var invoice = _invoiceRepository.GetInvoice(payment.Reference);

            string responseMessage;

            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            if (invoice.Amount == 0)
            {
                if (!invoice.PaymentsDone())
                    responseMessage = "no payment needed";
                else
                    throw new InvalidOperationException(
                        "The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }
            else
            {
                if (invoice.PaymentsDone())
                {
                    if (invoice.Amount == invoice.AmountPaid)
                    {
                        responseMessage = "invoice was already fully paid";
                    }
                    else if (invoice.AmountPaid != 0 && payment.Amount > invoice.Amount - invoice.AmountPaid)
                    {
                        responseMessage = "the payment is greater than the partial amount remaining";
                    }
                    else
                    {
                        if (invoice.Amount - invoice.AmountPaid == payment.Amount)
                        {
                            invoice.AddPayment(payment);
                            responseMessage = "final partial payment received, invoice is now fully paid";
                        }
                        else
                        {
                            invoice.AddPayment(payment);
                            responseMessage = "another partial payment received, still not fully paid";
                        }
                    }
                }
                else
                {
                    if (payment.Amount > invoice.Amount)
                    {
                        responseMessage = "the payment is greater than the invoice amount";
                    }
                    else if (invoice.Amount == payment.Amount)
                    {
                        invoice.AddPayment(payment);
                        responseMessage = "invoice is now fully paid";
                    }
                    else
                    {
                        invoice.AddPayment(payment);
                        responseMessage = "invoice is now partially paid";
                    }
                }
            }

            return responseMessage;
        }
    }
}