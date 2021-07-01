using Invoicing.Domain.Commands;
using System.Linq;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class PaymentLessThanRemainingAmount : IRule<AddPaymentCommand>
    {
        public bool IsSatisfied(AddPaymentCommand command)
        {
            var remainingAmount = command.Invoice.Amount - command.Invoice.AmountPaid;
            return command.Invoice.Payments.Any()
                && remainingAmount > command.Payment.Amount;
        }

        public bool IsValid(AddPaymentCommand command, out string message)
        {
            message = "another partial payment received, still not fully paid";
            return true;
        }
    }
}