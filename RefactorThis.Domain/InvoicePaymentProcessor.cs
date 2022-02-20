using System;
using System.Linq;
using RefactorThis.Persistence.model;
using RefactorThis.Persistence.repository;


namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;


        public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            var responseMessage = string.Empty;

            switch (inv)
            {
                case null:
                    throw new InvalidOperationException("There is no invoice matching this payment");
                default:
                {
                    switch (inv.Amount)
                    {
                        case 0:
                            responseMessage = inv.Payments == null || !inv.Payments.Any()
                                ? "no payment needed"
                                : throw new InvalidOperationException(
                                    "The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                            break;
                        default:
                        {
                            if (inv.Payments != null && inv.Payments.Any())
                            {
                                if (inv.Payments.Sum(x => x.Amount) != 0 &&
                                    inv.Amount == inv.Payments.Sum(x => x.Amount))
                                {
                                    responseMessage = "invoice was already fully paid";
                                }
                                else if (inv.Payments.Sum(x => x.Amount) != 0 &&
                                         payment.Amount > inv.Amount - inv.AmountPaid)
                                {
                                    responseMessage = "the payment is greater than the partial amount remaining";
                                }
                                else
                                {
                                    if (inv.Amount - inv.AmountPaid == payment.Amount)
                                    {
                                        inv.AmountPaid += payment.Amount;
                                        inv.Payments.Add(payment);
                                        responseMessage = "final partial payment received, invoice is now fully paid";
                                    }
                                    else
                                    {
                                        inv.AmountPaid += payment.Amount;
                                        inv.Payments.Add(payment);
                                        responseMessage = "another partial payment received, still not fully paid";
                                    }
                                }
                            }
                            else
                            {
                                if (payment.Amount > inv.Amount )
                                {
                                    responseMessage = "the payment is greater than the invoice amount";
                                }
                                else if (inv.Amount == payment.Amount && inv.Payments != null)
                                {
                                    inv.AmountPaid = payment.Amount;
                                    inv.Payments.Add(payment);
                                    responseMessage = "invoice is now fully paid";
                                }
                                else
                                {
                                    if (inv.Payments != null)
                                    {
                                        inv.AmountPaid = payment.Amount;
                                        inv.Payments.Add(payment);
                                        responseMessage = "invoice is now partially paid";
                                    }
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            inv.Save();

            return responseMessage;
        }

    }
}