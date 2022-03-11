using AutoFixture;
using Invoicing.Domain.Commands;
using Invoicing.Domain.Rules;
using Invoicing.Domain.Rules.Invoicing;

namespace Invoicing.Domain.Tests.Customizations
{
    public class InvoiceRuleEngineConcreteCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<AbstractRuleEngine<AddPaymentCommand>>(
                () => fixture.Create<InvoiceRuleEngine>());
        }
    }
}