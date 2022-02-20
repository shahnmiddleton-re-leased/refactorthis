using RefactorThis.Domain.PaymentRule.Interfaces;
using RefactorThis.Persistence.Model;
using RefactorThis.Persistence.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RefactorThis.Domain.PaymentRule
{
    public class PaymentRuleRepository : IPaymentRuleRepository
    {
        private List<Func<Invoice, Payment, IServiceResponse>> GetPaymentRules()
        {
            List<Func<Invoice, Payment, IServiceResponse>> listPaymentRules = new List<Func<Invoice, Payment, IServiceResponse>>();
            
            var paymentRuleType = typeof(IPaymentRule);
            var paymentRuleTypeInstances = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Where(x => paymentRuleType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                    .Select(x => Activator.CreateInstance(x) as IPaymentRule);
            listPaymentRules.AddRange(from paymentRule in paymentRuleTypeInstances
                                      select paymentRule.GetRule());
            return listPaymentRules;
        }

        public IServiceResponse ProcessPayment(Invoice invoice,Payment payment)
        {
            IServiceResponse responseMessage = null;
            List<Func<Invoice, Payment, IServiceResponse>> paymentRules = GetPaymentRules();
            foreach (var paymentRule in paymentRules)
            {
                var serviceResponse = paymentRule(invoice, payment);
                if (serviceResponse.IsErrorExist)
                {
                    responseMessage = serviceResponse;
                    if (serviceResponse.Exception != null)
                    {
                        throw serviceResponse.Exception;
                    }
                    break;
                }
                else if (!string.IsNullOrEmpty(serviceResponse.Message))
                {
                    responseMessage = serviceResponse;
                    break;
                }
            }

            return responseMessage;

        }
    }
}
