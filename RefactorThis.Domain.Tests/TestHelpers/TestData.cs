using System.Collections.Generic;
using RefactorThis.Domain.Model;

namespace RefactorThis.Domain.Tests.TestHelpers
{
    public class TestData
    {
        public static Invoice Invoice(decimal amount, decimal amountPaid, decimal? paymentAmount = null, string paymentReference = null)
        {
            var payments = paymentAmount.HasValue
                ? new List<Payment>(new List<Payment> {new Payment(paymentAmount.Value, paymentReference)})
                : new List<Payment>();

            return new Invoice(amount, amountPaid, payments);
        }
    }
}