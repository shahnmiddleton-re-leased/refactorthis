using RefactorThis.Domain.Constants;
using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;
using System.Linq;

namespace RefactorThis.Domain.Rule
{
    public class InvoicePaymentCompleteRule : IPaymentRule
    {
        public Func<Invoice, Payment, IServiceResponse> GetRule()
        {
            IServiceResponse paymentResponse = new PaymentResponse();
            return ((inv, pay) => {
                if (Convert.ToBoolean(inv?.Payments?.Any()) && inv.Amount>0 
                && inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
                {
                    paymentResponse.IsErrorExist = true;
                    paymentResponse.Message = PaymentMessage.InvoiceAlreadyPaid;
                    
                }
                return paymentResponse;
            });
        }
    }
}
