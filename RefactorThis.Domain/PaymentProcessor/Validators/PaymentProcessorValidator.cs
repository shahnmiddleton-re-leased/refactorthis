using System;
using System.Linq;
using RefactorThis.Domain.Common.ValidationModel;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.PaymentProcessor.Validators
{
    public class PaymentProcessorValidator : IPaymentProcessorValidator
    {
        private readonly IPaymentValidator _firstPaymentValidator;
        private readonly IPaymentValidator _additionalPaymentValidator;
        
        public PaymentProcessorValidator(IPaymentValidatorAbstractFactory paymentValidatorAbstractFactory)
        {
            _firstPaymentValidator = paymentValidatorAbstractFactory.GetFirstPaymentValidator();
            _additionalPaymentValidator = paymentValidatorAbstractFactory.GetAdditionalPaymentValidator();
        }

        public ValidationStatus Validate(Invoice invoice, Payment payment)
        {
            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            if (invoice.Amount == 0)
            {
                return NoPaymentRequired(invoice);
            }

            return IsFirstPayment(invoice) ?
                _firstPaymentValidator.Validate(invoice, payment) :
                _additionalPaymentValidator.Validate(invoice, payment);
        }

        private static ValidationStatus NoPaymentRequired(Invoice inv)
        {
            if (inv.Payments != null && inv.Payments.Any())
            {
                throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }

            return new ValidationStatus(false, "no payment needed");
        }

        private static bool IsFirstPayment(Invoice inv)
        {
            return inv.Payments == null || !inv.Payments.Any();
        }
    }
}
