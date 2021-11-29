﻿using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class NoPaymentsInvoiceIsFullyPaidRule : IPaymentRule
    {
        public PaymentResult RunRule( Invoice invoice, Payment payment )
        {
            var paymentResult = new PaymentResult( );

            if ( invoice.Payments.Any( ) ) return paymentResult;

            if ( payment.Amount == invoice.Amount )
            {
                paymentResult.ResponseMessage = "invoice is now fully paid";
                paymentResult.AddPayment = true;
                return paymentResult;
            }

            return paymentResult;
        }
    }
}
