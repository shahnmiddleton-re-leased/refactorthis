using Invoicing.Domain.Commands;
using Invoicing.Domain.Rules;
using System;
using System.Collections.Generic;

namespace Invoicing.Domain
{
    public class Invoice
    {
        private readonly IInvoiceRepository _repository;
        // TODO: for better separation, we can use notification / handler lib. i.e. MediatR
        private readonly AbstractRuleEngine<AddPaymentCommand> _addPaymentRules;
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        // TODO: should be readonly IReadOnlyCollection<Payment>, then need refactor the tests to separate classes
        private readonly List<Payment> _payments;
        public IList<Payment> Payments => _payments;

        // TODO: we can have tests asserting the ruleset injection into Invoice
        public Invoice(IInvoiceRepository repository, AbstractRuleEngine<AddPaymentCommand> addPaymentRules)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _addPaymentRules = addPaymentRules ?? throw new ArgumentNullException(nameof(addPaymentRules));
            _payments = new List<Payment>();
        }

        public string AddPayment(Payment payment)
        {
            if (payment is null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            var result = _addPaymentRules.Run(new AddPaymentCommand(this, payment));

            if (result.IsValid)
            {
                AmountPaid += payment.Amount;
                _payments.Add(payment);
            }

            return result.Message;
        }

        public void Save()
        {
            _repository.Update(this);
        }
    }
}