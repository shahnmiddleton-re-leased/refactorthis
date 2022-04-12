using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace RefactorThis.Domain.Model
{
	public class Invoice
	{
		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }

        public Result AddPayment(Payment payment)
        {
            if (Amount == 0)
            {
                if (Payments == null || !Payments.Any())
                {
                     return Result.Fail("no payment needed");
                }
                else
                {
                    return Result.Fail("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }

            if (Payments != null && Payments.Any())
            {
                if (Payments.Sum(x => x.Amount) != 0 && Amount == Payments.Sum(x => x.Amount))
                {
                    return Result.Fail("invoice was already fully paid");
                }
                else if (Payments.Sum(x => x.Amount) != 0 && payment.Amount > (Amount - AmountPaid))
                {
                    return Result.Fail("the payment is greater than the partial amount remaining");
                }
                else
                {
                    if ((Amount - AmountPaid) == payment.Amount)
                    {
                        AmountPaid += payment.Amount;
                        Payments.Add(payment);
                        return Result.Ok().WithSuccess("final partial payment received, invoice is now fully paid");
                    }
                    else
                    {
                        AmountPaid += payment.Amount;
                        Payments.Add(payment);
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
                    Payments.Add(payment);
                    return Result.Ok().WithSuccess("invoice is now fully paid");
                }
                else
                {
                    AmountPaid = payment.Amount;
                    Payments.Add(payment);
                    return Result.Ok().WithSuccess("invoice is now partially paid");
                }
            }
        }
    }
}