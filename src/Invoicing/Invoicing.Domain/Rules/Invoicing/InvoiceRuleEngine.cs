using Invoicing.Domain.Commands;
using System.Collections.Generic;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class InvoiceRuleEngine : AbstractRuleEngine<AddPaymentCommand>
    {
        // TODO: we can take this further and use IoC & generics to retrieve the rules
        public override IEnumerable<IRule<AddPaymentCommand>> GetRules()
        {
            yield return new NoPaymentNeeded();
            yield return new PaymentGreaterThanAmount();
            yield return new PaymentEqualsAmount();
            yield return new PaymentLessThanAmount();
            yield return new InvoiceAlreadyPaid();
            yield return new PaymentGreaterThanRemainingAmount();
            yield return new PaymentFinal();
            yield return new PaymentLessThanRemainingAmount();
        }
    }
}
