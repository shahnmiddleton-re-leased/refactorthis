using System;
using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class InvalidInvoiceRule : IPaymentRule
    {
        public PaymentResult RunRule( Invoice invoice, Payment payment )
        {
            var paymentResult = new PaymentResult( );

            if ( invoice.Amount == 0 && invoice.Payments != null && invoice.Payments.Any( ) )
            {
                throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
            }

            return paymentResult;
        }
    }
}
