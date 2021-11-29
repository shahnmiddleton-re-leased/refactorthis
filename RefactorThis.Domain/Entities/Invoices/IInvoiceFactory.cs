
namespace RefactorThis.Domain.Entities.Invoices
{
    /// <summary>
    /// Describes the Invoice Factory for creating invoice processors.
    /// </summary>
    public interface IInvoiceFactory
    {
        /// <summary>
        /// Creates an invoice payment processor.
        /// </summary>
        /// <returns>A new invoice payment processor.</returns>
        IInvoicePaymentProcessor CreateInvoicePaymentProcessor();
    }
}
