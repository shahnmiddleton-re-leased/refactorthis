using Invoicing.Domain.Commands;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class PaymentGreaterThanRemainingAmount : IRule<AddPaymentCommand>
    {
        public bool IsSatisfied(AddPaymentCommand command)
        {
            var remainingAmount = command.Invoice.Amount - command.Invoice.AmountPaid;
            return command.Payment.Amount > remainingAmount;
        }

        public bool IsValid(AddPaymentCommand command, out string message)
        {
            message = "the payment is greater than the partial amount remaining";
            return false;
        }
    }
}
