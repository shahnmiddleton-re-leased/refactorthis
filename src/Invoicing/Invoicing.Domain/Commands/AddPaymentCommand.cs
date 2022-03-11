using System;

namespace Invoicing.Domain.Commands
{
    public class AddPaymentCommand
    {
        public Invoice Invoice { get; }

        public Payment Payment { get; }

        public AddPaymentCommand(Invoice invoice, Payment payment)
        {
            Invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }
    }
}
