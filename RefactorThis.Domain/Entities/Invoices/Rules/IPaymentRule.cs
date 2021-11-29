
namespace RefactorThis.Domain.Entities.Invoices.Rules
{
    /// <summary>
    /// Describes a single payment rule for an invoice.
    /// </summary>
    public interface IPaymentRule
    {
        /// <summary>
        /// Processes an invoice payment rule.
        /// </summary>
        /// <param name="invoice">The invoice to apply the payment to.</param>
        /// <param name="payment">The payment to be applied.</param>
        /// <returns>A message if payment rule match found and processed.</returns>
        string ProcessPaymentRule(Invoice invoice, Payment payment);
    }
}
