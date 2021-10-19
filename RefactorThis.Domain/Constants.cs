namespace RefactorThis.Domain
{
    class Constants
    {
        // ---- constant placeholder
        // ---- Exceptions
        public const string noInvoice = "NoInvoice";
        public const string invalidState = "InvalidState";
        public const string partialPayment = "PartialPayment";
        public const string initialPayment = "InitialPayment";
        public const string descNoInvoice = "There is no invoice matching this payment";
        public const string descInvalidState = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
        public const string descSomethingWentWrong = "Something went wrong with the operation.";
        // ----- Messages
        public const string msgNoPaymentNeeded = "no payment needed";
        public const string msgInvoiceIsAlreadyPaid = "invoice was already fully paid";
        public const string msgPaymentIsGreaterThanPartialAmount = "the payment is greater than the partial amount remaining";
        public const string msgInvoicePartiallyPaidFullyPaid = "final partial payment received, invoice is now fully paid";
        public const string msgInvoicePartiallyPaidNotFullyPaid = "another partial payment received, still not fully paid";
        public const string msgPaymentIsGreaterThanInvoice = "the payment is greater than the invoice amount";
        public const string msgInvoiceFullyPaid = "invoice is now fully paid";
        public const string msgInvoicePartiallyPaid = "invoice is now partially paid";
    }
}
