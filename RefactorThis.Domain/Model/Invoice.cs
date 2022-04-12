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
        }

        public decimal Amount { get; private set; }
		public decimal AmountPaid { get; private set; }

        public decimal PaymentsRunningTotal => _payments.Sum(x => x.Amount);
        public bool HasPayments => _payments.Any();

        private readonly List<Payment> _payments = new List<Payment>();
        public IReadOnlyList<Payment> Payments => _payments;
        
        public Result AddPayment(Payment payment)
        {
            var validationResult = new InvoicePaymentValidator().IsValid(this, payment);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            if (HasPayments)
            {
                if ((Amount - AmountPaid) == payment.Amount)
                {
                    AmountPaid += payment.Amount;
                    _payments.Add(payment);
                    return Result.Ok().WithSuccess("final partial payment received, invoice is now fully paid");
                }

                AmountPaid += payment.Amount;
                _payments.Add(payment);
                return Result.Ok().WithSuccess("another partial payment received, still not fully paid");
            }

            if (Amount == payment.Amount)
            {
                AmountPaid = payment.Amount;
                _payments.Add(payment);
                return Result.Ok().WithSuccess("invoice is now fully paid");
            }

            AmountPaid = payment.Amount;
            _payments.Add(payment);
            return Result.Ok().WithSuccess("invoice is now partially paid");
        }
    }
}