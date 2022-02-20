using RefactorThis.Domain.Constants;
using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;

namespace RefactorThis.Domain.PaymentRule
{
    public class InvalidInvoiceRule : IPaymentRule
    {
        public Func<Invoice, Payment, IServiceResponse> GetRule()
        {
            IServiceResponse paymentResponse = new PaymentResponse();
            return ((inv, pay) => {
                if (inv == null)
                {
                    paymentResponse.IsErrorExist = true;
                    paymentResponse.Message = PaymentMessage.InvalidInvoice;
                    paymentResponse.Exception = new InvalidOperationException(PaymentMessage.InvalidInvoice);
                }
                return paymentResponse;
            });
        }
    }
}
