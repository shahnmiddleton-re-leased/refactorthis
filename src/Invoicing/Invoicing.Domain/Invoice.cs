using System;
using System.Collections.Generic;
using System.Linq;

namespace Invoicing.Domain
{
    public class Invoice
    {
        private readonly IInvoiceRepository _repository;
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        private readonly List<Payment> _payments;
        public IReadOnlyCollection<Payment> Payments => _payments;

        public Invoice(IInvoiceRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _payments = new List<Payment>();
        }

        public string AddPayment(Payment payment)
        {
            if (payment is null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            if (Amount == 0)
            {
                if (!_payments.Any())
                {
                    return "no payment needed";
                }
                else
                {
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }
            else
            {
                if (_payments.Any())
                {
                    if (_payments.Sum(x => x.Amount) != 0 && Amount == _payments.Sum(x => x.Amount))
                    {
                        return "invoice was already fully paid";
                    }
                    else if (_payments.Sum(x => x.Amount) != 0 && payment.Amount > (Amount - AmountPaid))
                    {
                        return "the payment is greater than the partial amount remaining";
                    }
                    else
                    {
                        if ((Amount - AmountPaid) == payment.Amount)
                        {
                            AmountPaid += payment.Amount;
                            _payments.Add(payment);
                            return "final partial payment received, invoice is now fully paid";
                        }
                        else
                        {
                            AmountPaid += payment.Amount;
                            _payments.Add(payment);
                            return "another partial payment received, still not fully paid";
                        }
                    }
                }
                else
                {
                    if (payment.Amount > Amount)
                    {
                        return "the payment is greater than the invoice amount";
                    }
                    else if (Amount == payment.Amount)
                    {
                        AmountPaid = payment.Amount;
                        _payments.Add(payment);
                        return "invoice is now fully paid";
                    }
                    else
                    {
                        AmountPaid = payment.Amount;
                        _payments.Add(payment);
                        return "invoice is now partially paid";
                    }
                }
            }
        }

        public void Save()
        {
            _repository.Update(this);
        }
    }
}