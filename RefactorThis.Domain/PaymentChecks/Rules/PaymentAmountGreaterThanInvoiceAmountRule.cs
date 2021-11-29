using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class PaymentAmountGreaterThanInvoiceAmountRule : IPaymentRule
    {
        public PaymentResult RunRule( Invoice invoice, Payment payment )
        {
            var paymentResult = new PaymentResult( );

            if ( invoice.Payments.Any( ) ) return paymentResult;

            if ( payment.Amount > invoice.Amount )
            {
                paymentResult.ResponseMessage = "the payment is greater than the invoice amount";
                paymentResult.AddPayment = true;
                return paymentResult;
            }

            return paymentResult;
        }
    }
}
