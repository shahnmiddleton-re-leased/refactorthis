using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessorService : IInvoicePaymentProcessorService
    {
        private readonly RefactorThisContext _refactorThisContext;
        private readonly IInternationalizationService _internationalizationService;

        public InvoicePaymentProcessorService(RefactorThisContext refactorThisContext, IInternationalizationService internationalizationService)
        {
            _refactorThisContext = refactorThisContext;
            _internationalizationService = internationalizationService;
        }

        public async Task<string> ProcessPaymentAsync(Payment payment)
        {
            var inv = await _refactorThisContext.Invoices.FindAsync(payment.InvoiceReference);
            if (inv == null)
            {
                throw new InvalidOperationException(_internationalizationService.GetTranslationFromKey(new InvoiceNotFound()));
            }

            if (inv.Amount == 0)
            {
                return inv.Payments.Any()
                    ? throw new InvalidOperationException(_internationalizationService.GetTranslationFromKey(new InvoiceInvalidState()))
                    : _internationalizationService.GetTranslationFromKey(new InvoiceNoPaymentNeeded());
            }

            var hasPreviousPayment = inv.Payments.Any();
            var remainingToPay = inv.Amount - inv.Payments.Sum(x => x.Amount);
            if (inv.Amount == inv.Payments.Sum(x => x.Amount))
            {
                return _internationalizationService.GetTranslationFromKey(new InvoiceAlreadyFullyPaid());
            }

            if (payment.Amount > remainingToPay)
            {
                return _internationalizationService.GetTranslationFromKey(hasPreviousPayment ? new InvoicePartialPaymentTooBig() : new InvoicePaymentTooBig());
            }

            inv.Payments.Add(payment);
            await _refactorThisContext.SaveChangesAsync();

            return _internationalizationService.GetTranslationFromKey((remainingToPay == payment.Amount, hasPreviousPayment) switch
            {
                (true, true) => new InvoiceFinalPaymentPaid(),
                (true, false) => new InvoiceFullyPaid(),
                (false, true) => new InvoicePartialPaymentReceived(),
                (false, false) => new InvoicePartiallyPaid(),
            });
        }
    }
}