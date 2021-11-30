using System;

namespace RefactorThis.Persistence
{
    /// <summary>
    /// A payment which has been made on an <see cref="Invoice"/>
    /// </summary>
    public class Payment
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="Payment"/> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="reference">The reference.</param>
        /// <exception cref="ArgumentException">Payment amount must be non-negative</exception>
        /// <exception cref="ArgumentException">Reference must be provided</exception>
        public Payment(decimal amount, string reference)
        {
            if (amount < 0M) throw new ArgumentException("Payment amount must be non-negative");
            if (string.IsNullOrEmpty(reference)) throw new ArgumentException("Reference must be provided");

            Amount = amount;
            PaymentReference = reference;
        }

        public decimal Amount { get; }
		public string PaymentReference { get; }
	}
}