using System.Linq;
using FluentResults;

namespace RefactorThis.Domain.Model
{
    public class InvoicePaymentValidator
    {
        public Result IsValid(Invoice invoice, Payment payment)
        {
            if (NoPaymentNeeded(invoice))
            {
                return Result.Fail("no payment needed");
            }

            if (PaymentsForZeroInvoiceAmount(invoice))
            {
                return Result.Fail("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }

            if (InvoiceAlreadyPaid(invoice))
            {
                return Result.Fail("invoice was already fully paid");
            }

            if (PaymentGreaterThanRemainingInvoiceAmount(invoice, payment))
            {
                return Result.Fail("the payment is greater than the partial amount remaining");
            }

            if (PaymentGreaterThanInvoiceAmount(invoice, payment))
            {
                return Result.Fail("the payment is greater than the invoice amount");
            }

            return Result.Ok();
        }

        private static bool PaymentGreaterThanInvoiceAmount(Invoice invoice, Payment payment)
        {
            return payment.Amount > invoice.Amount;
        }

        private static bool PaymentGreaterThanRemainingInvoiceAmount(Invoice invoice, Payment payment)
        {
            return invoice.PaymentsRunningTotal != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid);
        }

        private static bool InvoiceAlreadyPaid(Invoice invoice)
        {
            return invoice.PaymentsRunningTotal != 0 && invoice.Amount == invoice.PaymentsRunningTotal;
        }

        private static bool PaymentsForZeroInvoiceAmount(Invoice invoice)
        {
            return invoice.Amount == 0 && invoice.HasPayments;
        }

        private static bool NoPaymentNeeded(Invoice invoice)
        {
            return invoice.Amount == 0 && !invoice.HasPayments;
        }
    }
}