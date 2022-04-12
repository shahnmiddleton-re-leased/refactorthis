using RefactorThis.Domain.Model;
using TestStack.Dossier;
using TestStack.Dossier.EquivalenceClasses;

namespace RefactorThis.Domain.Tests.TestHelpers
{
    public class PaymentBuilder : TestDataBuilder<Payment, PaymentBuilder>
    {
        public PaymentBuilder()
        {
            Set(x => x.Amount, Any.PositiveInteger());
        }

        public PaymentBuilder WithAmount(decimal amount)
        {
            return Set(x => x.Amount, amount);
        }
    }
}
