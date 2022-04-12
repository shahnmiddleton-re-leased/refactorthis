using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace RefactorThis.Domain.Model
{
	public class Invoice
	{
        private Invoice() { }

        public Invoice(decimal amount, decimal amountPaid, List<Payment> payments)
        {
            Amount = amount;
            AmountPaid = amountPaid;
            _payments = payments ?? new List<Payment>();
            //  _payments = _payments is null ? new List<Payment>() : payments;
        }

        public decimal Amount { get; private set; }
		public decimal AmountPaid { get; private set; }

        private readonly List<Payment> _payments = new List<Payment>();
        public IReadOnlyList<Payment> Payments => _payments;
        
        public Result AddPayment(Payment payment)
        {
            if (Amount == 0)
            {
                if (_payments == null || !_payments.Any())
                {
                     return Result.Fail("no payment needed");
                }
                else
                {
                    return Result.Fail("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }

            if (_payments != null && _payments.Any())
            {
                if (_payments.Sum(x => x.Amount) != 0 && Amount == _payments.Sum(x => x.Amount))
                {
                    return Result.Fail("invoice was already fully paid");
                }
                else if (_payments.Sum(x => x.Amount) != 0 && payment.Amount > (Amount - AmountPaid))
                {
                    return Result.Fail("the payment is greater than the partial amount remaining");
                }
                else
                {
                    if ((Amount - AmountPaid) == payment.Amount)
                    {
                        AmountPaid += payment.Amount;
                        _payments.Add(payment);
                        return Result.Ok().WithSuccess("final partial payment received, invoice is now fully paid");
                    }
                    else
                    {
                        AmountPaid += payment.Amount;
                        _payments.Add(payment);
                        return Result.Ok().WithSuccess("another partial payment received, still not fully paid");
                    }
                }
            }
            else
            {
                if (payment.Amount > Amount)
                {
                    return Result.Fail("the payment is greater than the invoice amount");
                }
                else if (Amount == payment.Amount)
                {
                    AmountPaid = payment.Amount;
                    _payments.Add(payment);
                    return Result.Ok().WithSuccess("invoice is now fully paid");
                }
                else
                {
                    AmountPaid = payment.Amount;
                    _payments.Add(payment);
                    return Result.Ok().WithSuccess("invoice is now partially paid");
                }
            }
        }
    }
}