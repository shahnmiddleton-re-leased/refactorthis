using System.Linq;

namespace RefactorThis.Domain.PaymentHandlers
{
    public class PreProcessingHandler : IPaymentHandler
    {
        public void Handle(Payload payload)
        {
            if (payload.SkipRestHandlers) return;

            if (payload.Invoice.Payments?.Any() == true)
            {
                payload.InvoiceAlreadyHasPayment = true;
            }

            var gap = payload.Payment.Amount - (payload.Invoice.Amount - payload.Invoice.AmountPaid);

            if (gap > 0)
            {
                payload.CurrentInvoiceStatus = CurrentInvoiceStatus.ExceedInvoiceAmount;
            }
            else if (gap < 0)
            {
                payload.CurrentInvoiceStatus = CurrentInvoiceStatus.PartialPaid;
                payload.Invoice.AmountPaid += payload.Payment.Amount;
                payload.Invoice.Payments.Add(payload.Payment);
            }
            else
            {
                payload.CurrentInvoiceStatus = CurrentInvoiceStatus.FullyPaid;
                payload.Invoice.AmountPaid += payload.Payment.Amount;
                payload.Invoice.Payments.Add(payload.Payment);
            }
        }
    }
}
