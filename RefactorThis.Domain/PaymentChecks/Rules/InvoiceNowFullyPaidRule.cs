using System.Linq;
using RefactorThis.Domain.Interfaces;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.PaymentChecks.Rules
{
    public class InvoiceNowFullyPaidRule : IPaymentRule
    {
        public PaymentResult RunRule(Invoice invoice, Payment payment)
        {
            var paymentResult = new PaymentResult();

            if (!invoice.Payments.Any()) return paymentResult;
            
            if ((invoice.Amount - invoice.AmountPaid) == payment.Amount)
            {
                paymentResult.ResponseMessage = "final partial payment received, invoice is now fully paid";
                paymentResult.AddPayment = true;
                return paymentResult;
            }

            return paymentResult;
        }
    }
}
