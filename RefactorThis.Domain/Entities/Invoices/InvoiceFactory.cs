using RefactorThis.Domain.Entities.Invoices.Rules;
using System.Collections.Generic;

namespace RefactorThis.Domain.Entities.Invoices
{
    public class InvoiceFactory : IInvoiceFactory
    {
        public IInvoicePaymentProcessor CreateInvoicePaymentProcessor()
        {
            List<IPaymentRule> paymentRules = new List<IPaymentRule>
            {
                new InvoiceNoPaymentNeeded(),
                new InvoiceNoAmountWithPayments(),
                new InvoiceAlreadyPaid(),
                new InvoicePaymentLargerThanAmount(),
                new InvoiceFinalPartialPayment(),
                new InvoicePartialPaymentIncomplete(),
                new InvoicePaymentGreaterThanAmount(),
                new InvoiceFullyPaid(),
                new InvoicePartiallyPaid()
            };
            return new InvoicePaymentProcessor(paymentRules);
        }
    }
}
