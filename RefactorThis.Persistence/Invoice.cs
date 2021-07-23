using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
    /// <summary>
    /// An invoice, which has an <see cref="InvoiceNumber"/> and list of <see cref="Payment"/>s
    /// </summary>
    public class Invoice
    {
        private readonly List<Payment> _payments;

        /// <summary>
        /// Initializes a new instance of the <see cref="Invoice"/> class.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="amount">The amount.</param>
        public Invoice(string invoiceNumber, decimal amount)
        {
            _payments = new List<Payment>();

            InvoiceNumber = invoiceNumber;
            Amount = amount;
        }

        /// <summary>
        /// Gets the invoice number.
        /// </summary>
        /// <value>The invoice number.</value>
        public string InvoiceNumber { get; }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public decimal Amount { get; }

        /// <summary>
        /// Gets the amount paid.
        /// </summary>
        /// <value>The amount paid.</value>
        public decimal AmountPaid => Payments.Sum(x => x.Amount);

        /// <summary>
        /// Gets the balance.
        /// </summary>
        /// <value>The balance.</value>
        public decimal Balance => Amount - AmountPaid;

        /// <summary>
        /// Gets the payments.
        /// </summary>
        /// <value>The payments.</value>
        public IReadOnlyCollection<Payment> Payments => _payments;

        /// <summary>
        /// Makes a payment
        /// </summary>
        /// <param name="payment">The payment.</param>
        public void MakePayment(Payment payment)
        {
            _payments.Add(payment);

			// TODO: Log this to a central repository
			Console.WriteLine($"A payment of ${payment.Amount} has been made, with reference '{payment.PaymentReference}'");
        }

        /// <summary>
        /// Reverses a payment
        /// </summary>
        /// <param name="payment">The payment.</param>
        public void ReversePayment(Payment payment)
        {
            _payments.Remove(payment);

            // TODO: Log this to a central repository
            Console.WriteLine($"A payment of ${payment.Amount}, with reference '{payment.PaymentReference}' has been reversed and should be investigated");
        }
    }
}