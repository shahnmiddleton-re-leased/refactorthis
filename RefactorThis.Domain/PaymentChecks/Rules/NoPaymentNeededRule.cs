using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class NoPaymentNeededRule : IPaymentRule
    {
        public PaymentResult RunRule(Invoice invoice, Payment payment)
        {
            var paymentResult = new PaymentResult();

            if (invoice.Amount == 0 && (invoice.Payments == null || !invoice.Payments.Any()))
            {
                paymentResult.ResponseMessage = "no payment needed";
                paymentResult.AddPayment = false;
                return paymentResult;
            }

            return paymentResult;
        }
    }
}
