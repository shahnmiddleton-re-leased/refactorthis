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
            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            if (inv.Amount == 0)
            {
                return inv.Payments.Any()
                    ? throw new InvalidOperationException(
                        "The invoice is in an invalid state, it has an amount of 0 and it has payments.")
                    : "no payment needed";
            }
            
            var hasPreviousPayment = inv.Payments.Any();
            var remainingToPay = inv.Amount - inv.Payments.Sum(x => x.Amount);
            if (inv.Amount == inv.Payments.Sum(x => x.Amount))
            {
                return "invoice was already fully paid";
            }

            if (payment.Amount > remainingToPay)
            {
                return hasPreviousPayment ? "the payment is greater than the partial amount remaining" : "the payment is greater than the invoice amount";
            }

            inv.Payments.Add(payment);
            await _refactorThisContext.SaveChangesAsync();

            return (remainingToPay == payment.Amount, hasPreviousPayment) switch
            {
                (true, true) => "final partial payment received, invoice is now fully paid",
                (true, false) => "invoice is now fully paid",
                (false, true) => "another partial payment received, still not fully paid",
                (false, false) => "invoice is now partially paid",
            };
        }
    }
}