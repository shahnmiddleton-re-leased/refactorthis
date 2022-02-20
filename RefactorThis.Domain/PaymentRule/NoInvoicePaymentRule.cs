using RefactorThis.Domain.Constants;
using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace RefactorThis.Domain.PaymentRule
{
    public class NoInvoicePaymentRule : IPaymentRule
    {
        public Func<Invoice, Payment, IServiceResponse> GetRule()
        {
            IServiceResponse paymentResponse = new PaymentResponse();
            return ((inv, pay) => {
                if (inv?.Amount > 0 && !Convert.ToBoolean(inv.Payments?.Any()) && pay != null)
                {
                    if (pay.Amount > inv.Amount)
                    {
                        paymentResponse.IsErrorExist = true;
                        paymentResponse.Message = PaymentMessage.PaymentGreaterThanInvoice;
                    }
                    else if (inv.Amount == pay.Amount)
                    {
                        inv.AddPayment(pay);
                        paymentResponse.Message = PaymentMessage.InvoicePaymentComplete;
                    }
                    else
                    {
                        inv.AddPayment(pay);
                        paymentResponse.Message = PaymentMessage.InvoicePartiallyPaid;
                    }
                }
                return paymentResponse;
            });
        }
    }
}
