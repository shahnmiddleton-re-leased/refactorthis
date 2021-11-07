using RefactorThis.Domain.Common.ValidationModel;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.PaymentProcessor.Validators
{
    public class FirstPaymentValidator : IPaymentValidator
    {
        public ValidationStatus Validate(Invoice invoice, Payment payment)
        {
            if (PaymentGreaterThanAmount(invoice, payment))
            {
                return new ValidationStatus(false, "the payment is greater than the invoice amount");
            }
            
            return PaymentEqualsAmount(invoice, payment) ?
                new ValidationStatus(true, "invoice is now fully paid") :
                new ValidationStatus(true, "invoice is now partially paid");
        }

        private static bool PaymentEqualsAmount(Invoice invoice, Payment payment)
        {
            return invoice.Amount == payment.Amount;
        }

        private static bool PaymentGreaterThanAmount(Invoice invoice, Payment payment)
        {
            return payment.Amount > invoice.Amount;
        }
    }
}