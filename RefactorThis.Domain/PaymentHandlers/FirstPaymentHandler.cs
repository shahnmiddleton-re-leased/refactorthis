namespace RefactorThis.Domain.PaymentHandlers
{
    public class FirstPaymentHandler : IPaymentHandler
    {
        public void Handle(Payload payload)
        {
            if (payload.SkipRestHandlers) return;

            if (payload.InvoiceAlreadyHasPayment == true)
            {
                return;
            }
            else
            {
                switch (payload.CurrentInvoiceStatus)
                {
                    case CurrentInvoiceStatus.ExceedInvoiceAmount:
                        payload.Response = "the payment is greater than the invoice amount";
                        break;
                    case CurrentInvoiceStatus.FullyPaid:
                        payload.Response = "invoice is now fully paid";
                        break;
                    case CurrentInvoiceStatus.PartialPaid:
                        payload.Response = "invoice is now partially paid";
                        break;
                    default:
                        break;
                };
            }
        }
    }
}
