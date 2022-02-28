using System;
using System.Collections.Generic;
using System.Linq;
using RefactorThis.Domain.Abstractions.Messages;
using RefactorThis.Domain.Entities.Invoice.Events;
using RefactorThis.Domain.Entities.Invoice.Exceptions;

namespace RefactorThis.Domain.Entities.Invoice
{
    // aggregate root
    public partial class Invoice
    {
        public Invoice(decimal amount, decimal amountPaid, List<Payment> payments)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }

            var paymentsSum = payments.Sum(p => p.Amount);

            if (paymentsSum != amountPaid)
            {
                throw new ArgumentException("Total payments must equal amount paid.");
            }

            if (paymentsSum > amount)
            {
                throw new ArgumentException("Total payments must be less than or equal to amount.");
            }

            Amount = amount;
            AmountPaid = amountPaid;
            Payments = payments;
        }

        private decimal AmountOwing => Amount - AmountPaid;

        /// <summary>
        /// Processes a payment on an invoice.
        /// </summary>
        /// <param name="payment">The payment to process.</param>
        /// <returns>A domain event that occurs during this procedure.</returns>
        public IDomainEvent ProcessPayment(Payment payment)
        {
            // validate arguments
            if (payment.Amount <= 0)
            {
                throw new ArgumentException("Payment amount must be greater than zero.");
            }

            // invoice has already been paid in full
            if (AmountOwing == 0)
            {
                throw new NoPaymentRequiredException();
            }

            // payment amount is more than amount owing
            if (payment.Amount > AmountOwing)
            {
                if (Payments.Any())
                {
                    throw new PartialPaymentTooMuchException();
                }

                throw new FullPaymentTooMuchException();
            }

            // payment is valid, add the payment
            AddPayment(payment);

            return new PaymentProcessed(Payments.Count, AmountOwing == 0);
        }

        private void AddPayment(Payment payment)
        {
            AmountPaid += payment.Amount;
            Payments.Add(payment);
        }
    }
}
