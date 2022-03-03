namespace RefactorThis.Domain.Constants
{
    public static class PaymentResultMessage
    {
        public const string NoPaymentNeeded = "no payment needed";
        public const string InvoiceAlreadyFullyPaid = "invoice was already fully paid";

        private const string PartialPaymentExceedsRemainingBalance = "the payment is greater than the partial amount remaining";
        private const string FirstPaymentExceedsRemainingBalance = "the payment is greater than the invoice amount";
        public static string PaymentExceedsRemainingBalance(bool isFirstPayment) => isFirstPayment ? FirstPaymentExceedsRemainingBalance : PartialPaymentExceedsRemainingBalance;

        private const string PartialPaymentCompletedBalance = "final partial payment received, invoice is now fully paid";
        private const string PartialPaymentReceived = "another partial payment received, still not fully paid";
        public static string PaymentReceived(bool isFirstPayment) => isFirstPayment ? FirstPaymentReceived : PartialPaymentReceived;


        private const string FirstPaymentCompletedBalance = "invoice is now fully paid";
        private const string FirstPaymentReceived = "invoice is now partially paid";
        public static string PaymentComplete(bool isFirstPayment) => isFirstPayment ? FirstPaymentCompletedBalance : PartialPaymentCompletedBalance;

    }
}
