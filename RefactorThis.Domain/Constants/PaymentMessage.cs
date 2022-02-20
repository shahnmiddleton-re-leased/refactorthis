namespace RefactorThis.Domain.Constants
{
    public static class PaymentMessage
    {
        public const string InvoiceAlreadyPaid = "Invoice was already fully paid";
        public const string PaymentHighThanPartialAmount = "The payment is greater than the partial amount remaining";
        public const string CompletePayment = "Final partial payment received, invoice is now fully paid";
        public const string PartialPaymentComplete = "Another partial payment received, still not fully paid";
        public const string InvalidInvoice = "There is no invoice matching this payment";
        public const string NoPaymentRequired = "No payment required";
        public const string InvalidStateInvoice = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
        public const string PaymentGreaterThanInvoice = "The payment is greater than the invoice amount";
        public const string InvoicePaymentComplete = "Invoice is now fully paid";
        public const string InvoicePartiallyPaid = "Invoice is now partially paid";
    }
}
