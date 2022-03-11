using Invoicing.Domain.Commands;
using System.Linq;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class PaymentFinal : IRule<AddPaymentCommand>
    {
        public bool IsSatisfied(AddPaymentCommand command)
        {
            var remainingAmount = command.Invoice.Amount - command.Invoice.AmountPaid;
            return command.Invoice.Payments.Any()
                && remainingAmount == command.Payment.Amount;
        }

        public bool IsValid(AddPaymentCommand command, out string message)
        {
            message = "final partial payment received, invoice is now fully paid";
            return true;
        }
    }
}
