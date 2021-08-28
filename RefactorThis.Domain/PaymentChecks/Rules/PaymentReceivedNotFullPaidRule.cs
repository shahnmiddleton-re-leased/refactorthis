using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class PaymentReceivedNotFullPaidRule : IPaymentRule
    {
        public PaymentResult RunRule(Invoice invoice, Payment payment)
        {
            var paymentResult = new PaymentResult();

            if (!invoice.Payments.Any()) return paymentResult;

            var amountPaid = invoice.Payments.Sum(x => x.Amount);

            if (amountPaid != 0 && payment.Amount < amountPaid)
            {
                // Already fully Paid..
                paymentResult.ResponseMessage = "another partial payment received, still not fully paid";
                paymentResult.AddPayment = true;
                return paymentResult;
            }

            return paymentResult;
        }
    }
}
