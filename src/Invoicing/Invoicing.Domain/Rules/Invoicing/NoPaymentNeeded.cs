using Invoicing.Domain.Commands;
using System;
using System.Linq;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class NoPaymentNeeded : IRule<AddPaymentCommand>
    {
        public bool IsSatisfied(AddPaymentCommand command)
        {
            return command.Invoice.Amount == 0;
        }

        public bool IsValid(AddPaymentCommand command, out string message)
        {
            if(command.Invoice.Payments.Any())
            {
                throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }

            message = "no payment needed";
            return false;
        }
    }
}
