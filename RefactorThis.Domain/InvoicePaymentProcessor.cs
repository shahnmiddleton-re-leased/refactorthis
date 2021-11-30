using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    /// <summary>
    /// The Invoice Payment Processor
    /// </summary>
    public static class InvoicePaymentProcessor
	{
        /// <summary>
        /// Processes a payment for the specified invoice number.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="payment">The payment.</param>
        /// <returns>The result of the processed payment</returns>
        public static InvoicePaymentResult Process(string invoiceNumber, Payment payment)
        {
            var invoice = InvoiceRepository.Instance.GetInvoiceByInvoiceNumber(invoiceNumber);

            if (invoice is null) return InvoicePaymentResult.NotPaid_NoMatchingInvoice;
            if (invoice.Amount == 0M) return InvoicePaymentResult.NotPaid_NoPaymentNeeded;
            if (invoice.Balance == 0M) return InvoicePaymentResult.NotPaid_AlreadyFullyPaid;
            if (payment.Amount > invoice.Balance) return InvoicePaymentResult.NotPaid_Overpayment;

			invoice.MakePayment(payment);

            if (!InvoiceRepository.Instance.SaveInvoice(invoice))
            {
                invoice.ReversePayment(payment);

                return InvoicePaymentResult.NotPaid_FailedToSaveToDatabase;
            }

            return invoice.Balance > 0M
                ? InvoicePaymentResult.Paid_PartialAmountRemaining
                : InvoicePaymentResult.Paid_InvoiceFullyPaid;
        }
	}
}