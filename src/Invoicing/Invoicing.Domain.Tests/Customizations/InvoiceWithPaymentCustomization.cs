using AutoFixture;

namespace Invoicing.Domain.Tests.Customizations
{
    public class InvoiceWithPaymentCustomization : ICustomization
    {
        private readonly decimal _amount;
        private readonly decimal _amountPaid;
        private readonly decimal _paymentAmount;

        public InvoiceWithPaymentCustomization(
            decimal amount,
            decimal amountPaid,
            decimal paymentAmount)
        {
            _amount = amount;
            _amountPaid = amountPaid;
            _paymentAmount = paymentAmount;
        }

        public void Customize(IFixture fixture)
        {
            fixture
                .Register(() =>
                    {
                        var invoice = new Invoice(fixture.Create<IInvoiceRepository>())
                        {
                            Amount = _amount,
                            AmountPaid = _amountPaid
                        };

                        invoice.AddPayment(new Payment { Amount = _paymentAmount });
                        return invoice;
                    }); ;
        }
    }
}