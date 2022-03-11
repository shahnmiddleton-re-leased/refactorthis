using Invoicing.Domain.Commands;
using System.Linq;

namespace Invoicing.Domain.Rules.Invoicing
{
    public class InvoiceAlreadyPaid : IRule<AddPaymentCommand>
    {
        public bool IsSatisfied(AddPaymentCommand command)
        {
            return command.Invoice.Payments.Sum(p => p.Amount) == command.Invoice.Amount;
        }

        public bool IsValid(AddPaymentCommand command, out string message)
        {
            message = "invoice was already fully paid";
            return false;
        }
    }
}
