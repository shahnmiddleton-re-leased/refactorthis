using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class PaymentIsGreaterThanRemainingAmountRule : IPaymentRule
    {
        public PaymentResult RunRule( Invoice invoice, Payment payment )
        {
            var paymentResult = new PaymentResult( );

            if ( !invoice.Payments.Any( ) ) return paymentResult;

            var amountPaid = invoice.Payments.Sum( x => x.Amount );
            var amountRemaining = invoice.Amount - invoice.AmountPaid;

            if ( amountPaid != 0 && payment.Amount > amountRemaining )
            {
                paymentResult.ResponseMessage = "the payment is greater than the partial amount remaining";
                paymentResult.AddPayment = false;
                return paymentResult;
            }

            return paymentResult;
        }
    }
}
