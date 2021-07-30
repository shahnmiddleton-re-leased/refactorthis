using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessorService : IInvoicePaymentProcessorService
    {
        private readonly RefactorThisContext _refactorThisContext;

        public InvoicePaymentProcessorService(RefactorThisContext refactorThisContext)
        {
            _refactorThisContext = refactorThisContext;
        }

        public async Task<string> ProcessPaymentAsync(Payment payment)
        {
            var inv = await _refactorThisContext.Invoices.FindAsync(payment.InvoiceReference);

            var responseMessage = string.Empty;

            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            if (inv.Amount == 0)
            {
                if (!inv.Payments.Any())
                {
                    responseMessage = "no payment needed";
                }
                else
                {
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }
            else
            {
                if (inv.Payments.Any())
                {
                    if (inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
                    {
                        responseMessage = "invoice was already fully paid";
                    }
                    else if (inv.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (inv.Amount - inv.AmountPaid))
                    {
                        responseMessage = "the payment is greater than the partial amount remaining";
                    }
                    else
                    {
                        if ((inv.Amount - inv.AmountPaid) == payment.Amount)
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
                    if (payment.Amount > inv.Amount)
                    {
                        responseMessage = "the payment is greater than the invoice amount";
                    }
                    else if (inv.Amount == payment.Amount)
                    {
                        inv.AmountPaid = payment.Amount;
                        inv.Payments.Add(payment);
                        responseMessage = "invoice is now fully paid";
                    }
                    else
                    {
                        inv.AmountPaid = payment.Amount;
                        inv.Payments.Add(payment);
                        responseMessage = "invoice is now partially paid";
                    }
                }
            }

            await _refactorThisContext.SaveChangesAsync();

            return responseMessage;
        }
    }
}