using System;
using System.Linq;

namespace RefactorThis.Domain.PaymentHandlers
{
    public class ValidationHandler : IPaymentHandler
    {
        public void Handle(Payload payload)
        {
            if (payload?.Invoice is null || payload?.Payment is null)
            {
                throw new ArgumentNullException();
            }


            payload.SkipRestHandlers = true;
            if (payload.Invoice.AmountPaid != (payload.Invoice.Payments?.Sum(x => x.Amount) ?? 0))
            {
                payload.Response = "Invoice amount and payments not matching";
                return;
            }

            if (payload.Invoice.Amount == 0)
            {
                if (payload.Invoice.Payments == null || !payload.Invoice.Payments.Any())
                {
                    payload.Response = "no payment needed";
                }
                else
                {
                    payload.Response = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
                }
                return;
            }

            if (payload.Invoice.Amount == payload.Invoice.AmountPaid)
            {
                payload.Response = "invoice was already fully paid";
                return;
            }

            payload.SkipRestHandlers = false;
            payload.IsValid = true;
        }
    }
}
