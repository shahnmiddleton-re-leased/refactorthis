using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
    public class Invoice
    {
        private readonly List<Payment> _payments;

        public Invoice(decimal amount, string reference, Payment payment = null)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("The amount is invalid!");
            }

            _payments = new List<Payment>();
            Amount = amount;
            Reference = reference;
            if (payment != null)
            {
                AddPayment(payment);
            }
        }

        public void AddPayment(Payment payment)
        {
            if (!Reference.Equals(payment.Reference))
            {
                throw new InvalidOperationException("Payment reference does not match the invoice!");
            }

            _payments.Add(payment);
            AmountPaid += payment.Amount;
        }

        public bool PaymentsDone()
        {
            return _payments.Any();
        }

        public decimal Amount { get; }
        public decimal AmountPaid { get; private set; }

        public readonly string Reference;
    }
}