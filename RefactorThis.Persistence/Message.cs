using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence
{
    public class ErrorMessages
    {
        public const string DATA_INTEGRITY_NO_INVOICE_MATCH = "There is no invoice matching this payment";
        public const string DATA_INTEGRITY_INVALID_INVOICE_AMOUNT_0 = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
    }


    public class ResponseMessages
    {
        public const string INVOICE_NO_PAYMENT_NEEDED = "No payment needed";
        public const string INVOICE_ALREADY_PAID = "Invoice was already fully paid";
        public const string INVOICE_PAID_FULLY = "invoice is now fully paid";
        public const string INVOICE_PAID_PARTIALLY = "invoice is now partially paid";
        public const string INVOICE_PAID_GREATER_THEN_REMAINING = "the payment is greater than the partial amount remaining";
        public const string INVOICE_PAID_GREATER_THEN_TOTAL = "the payment is greater than the invoice amount";
        public const string INVOICE_AMOUNT_PARTIAL_PAYMENT_FULLY_PAID = "final partial payment received, invoice is now fully paid";
        public const string INVOICE_AMOUNT_PAID_PARTIALLY = "another partial payment received, still not fully paid";


    }
}
