using System.Linq;
using RefactorThis.Domain.Common.ValidationModel;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.PaymentProcessor.Validators
{
    public class AdditionalPaymentValidator : IPaymentValidator
    {
        public ValidationStatus Validate(Invoice invoice, Payment payment)
        {
            if (InvoiceIsPaid(invoice))
            {
                return new ValidationStatus(false, "invoice was already fully paid");
            }
            
            if (PaymentGreaterThanAmountRemaining(payment, invoice))
            {
                return new ValidationStatus(false, "the payment is greater than the partial amount remaining");
            }

            return IsFinalPayment(payment, invoice) ?
                new ValidationStatus(true, "final partial payment received, invoice is now fully paid") :
                new ValidationStatus(true, "another partial payment received, still not fully paid");
        }

        private static bool InvoiceIsPaid(Invoice invoice)
        {
            return invoice.Payments.Sum(x => x.Amount) != 0 && invoice.Amount == invoice.Payments.Sum(x => x.Amount);
        }

        private static bool PaymentGreaterThanAmountRemaining(Payment payment, Invoice invoice)
        {
            return invoice.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid);
        }

        private static bool IsFinalPayment(Payment payment, Invoice invoice)
        {
            return (invoice.Amount - invoice.AmountPaid) == payment.Amount;
        }
    }
}