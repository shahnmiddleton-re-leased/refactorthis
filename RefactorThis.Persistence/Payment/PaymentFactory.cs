using System.Linq;

namespace RefactorThis.Persistence.Payment
{
    public static class PaymentFactory
    {
        public static IPayment GetPaymentType(Invoice invoice, decimal amount)
        {
            IPayment paymentType = null;
            
            bool hasPayments = invoice.Payments != null && invoice.Payments.Any();
            if (hasPayments)
            {
                bool isPaymentsTotalNotZero = invoice.Payments.Sum(x => x.Amount) != 0;
                bool isInvoiceFullyPaid = isPaymentsTotalNotZero && invoice.Amount == invoice.Payments.Sum(x => x.Amount);
                bool isPaymentGreaterThanAmountRemaining = isPaymentsTotalNotZero && amount > (invoice.Amount - invoice.AmountPaid);

                if (isInvoiceFullyPaid)
                {
                    paymentType = new FullyPaidPayment();
                }
                else if (isPaymentGreaterThanAmountRemaining)
                {
                    paymentType = new InvalidAdditionalPayment();
                }
                else
                {
                    paymentType = new AdditionalPayment(invoice);
                }
            }
            else
            {
                paymentType = (amount > invoice.Amount ? (IPayment) new InvalidInitialPayment() : new InitialPayment(invoice));       
            }
            
            return paymentType;
        }
    }
}
