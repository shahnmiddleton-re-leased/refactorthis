using Invoicing.Domain.Commands;
using System.Linq;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class PaymentEqualsAmount : IRule<AddPaymentCommand>
    {
        public bool IsSatisfied(AddPaymentCommand command)
        {
            return !command.Invoice.Payments.Any()
                && command.Invoice.Amount == command.Payment.Amount;
        }

        public bool IsValid(AddPaymentCommand command, out string message)
        {
            message = "invoice is now fully paid";
            return true;
        }
    }
}
