using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Domain.InternationalizationService;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.InvoicePaymentProcessorService
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

            var hasPreviousPayment = inv.Payments.Any();
            var remainingToPay = inv.Amount - inv.Payments.Sum(x => x.Amount);
            var isFullPayment = remainingToPay == payment.Amount;

            if (inv.Amount == 0)
            {
                return hasPreviousPayment
                    ? throw new InvalidOperationException(_internationalizationService.GetTranslationFromKey(new InvoiceInvalidState()))
                    : _internationalizationService.GetTranslationFromKey(new InvoiceNoPaymentNeeded());
            }

            if (remainingToPay == 0)
            {
                return _internationalizationService.GetTranslationFromKey(new InvoiceAlreadyFullyPaid());
            }

            if (payment.Amount > remainingToPay)
            {
                return _internationalizationService.GetTranslationFromKey(hasPreviousPayment ? new InvoicePartialPaymentTooBig() : new InvoicePaymentTooBig());
            }

            inv.Payments.Add(payment);
            await _refactorThisContext.SaveChangesAsync();

            return _internationalizationService.GetTranslationFromKey((isFullPayment, hasPreviousPayment) switch
            {
                (true, true) => new InvoiceFinalPaymentPaid(),
                (true, false) => new InvoiceFullyPaid(),
                (false, true) => new InvoicePartialPaymentReceived(),
                (false, false) => new InvoicePartiallyPaid(),
            });
        }
    }
}