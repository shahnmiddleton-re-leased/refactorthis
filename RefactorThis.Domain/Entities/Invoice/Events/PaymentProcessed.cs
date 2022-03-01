using RefactorThis.Domain.Abstractions.Messages;

namespace RefactorThis.Domain.Entities.Invoice.Events
{
    public class PaymentProcessed : IDomainEvent
    {
        public int PaymentCount { get; private set; }
        public bool IsPaidInFull { get; private set; }

        public PaymentProcessed(int paymentCount, bool isPaidInFull)
        {
            PaymentCount = paymentCount;
            IsPaidInFull = isPaidInFull;
        }
    }
}
