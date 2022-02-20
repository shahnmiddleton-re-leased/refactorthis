using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain.PaymentRule.Interfaces
{
    public interface IPaymentRuleRepository
    {
        /// <summary>
        /// Process the Payment 
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        IServiceResponse ProcessPayment(Invoice invoice, Payment payment);
    }
}
