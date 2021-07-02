using Invoicing.Domain.Commands;
using System.Linq;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class PaymentGreaterThanAmount : IRule<AddPaymentCommand>
    {
        public bool IsSatisfied(AddPaymentCommand command)
        {
            return !command.Invoice.Payments.Any()
                && command.Payment.Amount > command.Invoice.Amount;
        }

        public bool IsValid(AddPaymentCommand command, out string message)
        {
            message = "the payment is greater than the invoice amount";
            return false;
        }
    }
}
