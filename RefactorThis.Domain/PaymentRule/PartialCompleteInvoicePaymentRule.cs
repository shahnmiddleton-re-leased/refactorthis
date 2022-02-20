using RefactorThis.Domain.Constants;
using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;
using System.Linq;

namespace RefactorThis.Domain.PaymentRule
{
    public class PartialCompleteInvoicePaymentRule : IPaymentRule
    {
        public Func<Invoice, Payment, IServiceResponse> GetRule()
        {
            IServiceResponse paymentResponse = new PaymentResponse();
            return ((inv, pay) => {
                if (Convert.ToBoolean(inv?.Payments?.Any()) && inv.Amount>0 && pay!=null &&
                ((inv.Amount != inv.Payments.Sum(x => x.Amount) && pay.Amount <= (inv.Amount - inv.AmountPaid))))
                {
                    paymentResponse.Message = ((inv.Amount - inv.AmountPaid) == pay.Amount) ? PaymentMessage.CompletePayment
                    : PaymentMessage.PartialPaymentComplete;
                    inv.AddPayment(pay);

                }
                return paymentResponse;
            });
        }
    }
}
