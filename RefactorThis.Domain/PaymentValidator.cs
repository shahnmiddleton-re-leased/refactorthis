using RefactorThis.Persistence;
using System;

namespace RefactorThis.Domain
{
    public class PaymentValidator
    {
        public void PreValidatePayment(Payment payment)
        {
            if (string.IsNullOrWhiteSpace(payment.Reference))
            {
                throw new InvalidOperationException("Invalid payment: reference is required");
            }
        }
    }
}
