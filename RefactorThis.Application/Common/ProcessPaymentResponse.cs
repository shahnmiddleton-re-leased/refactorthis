namespace RefactorThis.Application.Common
{
    public static class ProcessPaymentResponse
    {
        //public static string NoPaymentNeeded = "no payment needed";
        public static string InvoiceAlreadyPaid = "invoice was already fully paid";
        public static string PartialPaymentGreaterThanInvoiceAmount = "the payment is greater than the partial amount remaining";
        public static string PartialPaymentInvoiceFullyPaid = "final partial payment received, invoice is now fully paid";
        public static string PartialPaymentInvoicePartiallyPaid = "another partial payment received, still not fully paid";
        public static string PaymentGreaterThanInvoiceAmount = "the payment is greater than the invoice amount";
        public static string InvoiceFullyPaid = "invoice is now fully paid";
        public static string InvoicePartiallyPaid = "invoice is now partially paid";
    }
}
