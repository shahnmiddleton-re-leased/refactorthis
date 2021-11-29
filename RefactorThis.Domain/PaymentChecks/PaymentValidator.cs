using System.Collections.Generic;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Domain.PaymentChecks.Rules;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks
{
    public class PaymentValidator : IPaymentValidator
    {
        private readonly IList<IPaymentRule> _paymentRules;

        public PaymentValidator( )
        {
            _paymentRules = new List<IPaymentRule>
            {
                new NoPaymentNeededRule( ),
                new InvalidInvoiceRule( ),
                new InvoiceAlreadyFullyPaidRule( ),
                new PaymentIsGreaterThanRemainingAmountRule( ),
                new InvoiceNowFullyPaidRule( ),
                new PaymentReceivedNotFullPaidRule( ),
                new PaymentAmountGreaterThanInvoiceAmountRule( ),
                new NoPaymentsInvoiceIsFullyPaidRule( ),
                new NoPaymentsInvoiceIsPartiallyPaidRule( ),
            };
        }

        public PaymentResult Validate( Invoice inv, Payment payment )
        {
            var paymentResult = new PaymentResult( );
            foreach ( var paymentRule in _paymentRules )
            {
                paymentResult = paymentRule.RunRule( inv, payment );
                if ( paymentResult.HasConditionMet )
                {
                    return paymentResult;
                }
            }

            return paymentResult;
        }
    }
}
