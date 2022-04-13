using System.Collections.Generic;
using System.Linq;
using FluentResults;
using RefactorThis.Domain.Model.Invoices.Rules;

namespace RefactorThis.Domain.Model.Invoices
{
    public class InvoicePaymentValidator
    {
        private readonly List<IPaymentRule> _rules = new List<IPaymentRule>();

        public InvoicePaymentValidator()
        {
            _rules.Add(new NoPaymentNeeded());
            _rules.Add(new PaymentsForZeroInvoiceAmount());
            _rules.Add(new InvoiceAlreadyPaid());
            _rules.Add(new PaymentGreaterThanRemainingInvoiceAmount());
            _rules.Add(new PaymentGreaterThanInvoiceAmount());
        }

        public Result IsValid(Invoice invoice, Payment payment)
        {
            foreach (var rule in _rules.Where(rule => rule.MatchesFor(invoice, payment)))
            {
                return Result.Fail(rule.ValidationMessage);
            }

            return Result.Ok();
        }

    }
}