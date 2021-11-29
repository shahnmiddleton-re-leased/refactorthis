
namespace RefactorThis.Domain.Entities.Invoices
{
    /// <summary>
    /// Handles invoice payments.
    /// </summary>
    public interface IInvoicePaymentProcessor
    {
        /// <summary>
        /// Processes a payment against an associated invoice.
        /// </summary>
        /// <param name="invoice">The invoice to apply payment.</param>
        /// <param name="payment">Payment to add to associated invoice.</param>
        /// <returns>The processed payment message.</returns>
        string ProcessPayment(Invoice invoice, Payment payment);
    }
}
