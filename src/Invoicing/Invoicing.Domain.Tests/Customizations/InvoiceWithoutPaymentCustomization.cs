using AutoFixture;

namespace Invoicing.Domain.Tests.Customizations
{
    public class InvoiceWithoutPaymentCustomization : ICustomization
    {
        private readonly decimal _amount;
        private readonly decimal _amountPaid;

        public InvoiceWithoutPaymentCustomization(decimal amount, decimal amountPaid)
        {
            _amount = amount;
            _amountPaid = amountPaid;
        }

        public void Customize(IFixture fixture)
        {
            fixture
                .Customize<Invoice>(c => c
                    .With(i => i.Amount, _amount)
                    .With(i => i.AmountPaid, _amountPaid));
        }
    }
}
