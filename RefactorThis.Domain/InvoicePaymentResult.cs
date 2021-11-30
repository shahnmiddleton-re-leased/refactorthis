namespace RefactorThis.Domain
{
    /// <summary>
    /// The result of the <see cref="InvoicePaymentProcessor"/>
    /// </summary>
    public enum InvoicePaymentResult
    {
        /// <summary>
        /// Payment was made, there is still an outstanding amount remaining to pay
        /// </summary>
        Paid_PartialAmountRemaining,

        /// <summary>
        /// Payment was made, the invoice has now been fully paid
        /// </summary>
        Paid_InvoiceFullyPaid,

        /// <summary>
        /// A payment was not made, as no matching invoice was found
        /// </summary>
        NotPaid_NoMatchingInvoice,

        /// <summary>
        /// A payment was not made, as the amount of the invoice was zero
        /// </summary>
        NotPaid_NoPaymentNeeded,

        /// <summary>
        /// A payment was not made, as the invoice is already fully paid
        /// </summary>
        NotPaid_AlreadyFullyPaid,

        /// <summary>
        /// A payment was not made, as the payment amount was greater than the balance of the invoice
        /// </summary>
        NotPaid_Overpayment,

        /// <summary>
        /// A payment was not made, as an error occurred when saving the invoice to the database
        /// </summary>
        NotPaid_FailedToSaveToDatabase,
    }
}