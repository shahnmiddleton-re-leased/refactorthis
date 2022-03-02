namespace RefactorThis.Domain.PaymentHandlers
{
    public class PartialPaymentHandler : IPaymentHandler
    {
        public void Handle(Payload payload)
        {
            if (payload.SkipRestHandlers) return;

            if (!payload.InvoiceAlreadyHasPayment)
            {
                return;
            }
            else
            {
                switch (payload.CurrentInvoiceStatus)
                {
                    case CurrentInvoiceStatus.ExceedInvoiceAmount:
                        payload.Response = "the payment is greater than the partial amount remaining";
                        break;
                    case CurrentInvoiceStatus.FullyPaid:
                        payload.Response = "final partial payment received, invoice is now fully paid";
                        break;
                    case CurrentInvoiceStatus.PartialPaid:
                        payload.Response = "another partial payment received, still not fully paid";
                        break;
                    default:
                        break;
                };
            }
        }
    }
}
