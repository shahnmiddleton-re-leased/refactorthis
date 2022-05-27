using System;

namespace RefactorThis.Persistence
{
    public class Payment
    {
        public Payment(decimal amount, string reference)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("The payment has an invalid amount set!");
            }

            Amount = amount;
            Reference = reference;
        }

        public decimal Amount { get; }

        public string Reference { get; }
    }
}