using RefactorThis.Domain.Constants;
using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;
using System.Linq;

namespace RefactorThis.Domain.PaymentRule
{
    public class OverPaymentThanInvoiceRule : IPaymentRule
    {
        public Func<Invoice, Payment, IServiceResponse> GetRule()
        {
            IServiceResponse paymentResponse = new PaymentResponse();
            return ((inv, pay) => {
               
                if (Convert.ToBoolean(inv?.Payments?.Any()) && inv.Amount>0 && pay != null 
                && inv.Payments.Sum(x => x.Amount) != 0 && pay.Amount > (inv.Amount - inv.AmountPaid))
                {
                    paymentResponse.IsErrorExist = true;
                    paymentResponse.Message = PaymentMessage.PaymentHighThanPartialAmount;
                }
                
                return paymentResponse;
            });
        }
    }
}
