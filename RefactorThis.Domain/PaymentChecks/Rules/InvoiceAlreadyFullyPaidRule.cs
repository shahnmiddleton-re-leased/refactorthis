using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class InvoiceAlreadyFullyPaidRule : IPaymentRule
    {
        public PaymentResult RunRule(Invoice invoice, Payment payment)
        {
            var paymentResult = new PaymentResult();

            if (!invoice.Payments.Any()) return paymentResult;

            var amountPaid = invoice.Payments.Sum(x => x.Amount);

            if (amountPaid != 0 && invoice.Amount == amountPaid)
            {
                // Already fully Paid..
                paymentResult.ResponseMessage = "invoice was already fully paid";
                paymentResult.AddPayment = false;
                return paymentResult;
            }

            return paymentResult;
        }
    }
}
