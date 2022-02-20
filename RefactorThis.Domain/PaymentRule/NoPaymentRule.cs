using RefactorThis.Domain.Constants;
using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;
using System.Linq;

namespace RefactorThis.Domain.PaymentRule
{
    public class NoPaymentRule : IPaymentRule
    {
        public Func<Invoice, Payment, IServiceResponse> GetRule()
        {
            IServiceResponse paymentResponse = new PaymentResponse();
            return ((inv, pay) => {
                if (inv!=null && inv.Amount == 0)
                {
                    if (inv.Payments == null || !inv.Payments.Any())
                    {
                        paymentResponse.IsErrorExist = true;
                        paymentResponse.Message = PaymentMessage.NoPaymentRequired;
                    }
                    else
                    {
                        paymentResponse.IsErrorExist = true;
                        paymentResponse.Message = PaymentMessage.InvalidStateInvoice;
                        paymentResponse.Exception = new InvalidOperationException(PaymentMessage.InvalidStateInvoice);
                    }
                }
                return paymentResponse;
            });
        }
    }
}
