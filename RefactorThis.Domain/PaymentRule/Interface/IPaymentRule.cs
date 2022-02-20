using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;

namespace RefactorThis.Domain.PaymentRule.Interfaces
{
    public interface IPaymentRule
    {
        /// <summary>
        /// Get Payment Rule excuted for provided Invoice and Payment
        /// </summary>
        /// <returns></returns>
        Func<Invoice, Payment, IServiceResponse> GetRule();
    }
}
