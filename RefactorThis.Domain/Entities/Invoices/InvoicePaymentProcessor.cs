using RefactorThis.Domain.Entities.Invoices.Rules;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain.Entities.Invoices
{
    internal class InvoicePaymentProcessor : IInvoicePaymentProcessor
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<IPaymentRule> _paymentRules;

        public InvoicePaymentProcessor(List<IPaymentRule> paymentRules)
        {
            _paymentRules = paymentRules;
        }

        public string ProcessPayment(Invoice invoice, Payment payment)
        {
            if (invoice is null)
                throw new InvalidOperationException("There is no invoice matching this payment");

            foreach (var rule in _paymentRules)
            {
                string result = rule.ProcessPaymentRule(invoice, payment);
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            Logger.Error("No Invoice Payment Rule foun for Invoice {invoice} Payment {payment}", invoice, payment);
            return null;
        }
    }
}