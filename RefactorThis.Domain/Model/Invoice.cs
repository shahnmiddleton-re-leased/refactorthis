using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Domain.Model
{
	public class Invoice
	{
		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public List<Payment> Payments { get; set; }

        public string AddPayment(Payment payment)
        {
            var responseMessage = string.Empty;

            if (Amount == 0)
            {
                if (Payments == null || !Payments.Any())
                {
                    responseMessage = "no payment needed";
                }
                else
                {
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }
            else
            {
                if (Payments != null && Payments.Any())
                {
                    if (Payments.Sum(x => x.Amount) != 0 && Amount == Payments.Sum(x => x.Amount))
                    {
                        responseMessage = "invoice was already fully paid";
                    }
                    else if (Payments.Sum(x => x.Amount) != 0 && payment.Amount > (Amount - AmountPaid))
                    {
                        responseMessage = "the payment is greater than the partial amount remaining";
                    }
                    else
                    {
                        if ((Amount - AmountPaid) == payment.Amount)
                        {
                            AmountPaid += payment.Amount;
                            Payments.Add(payment);
                            responseMessage = "final partial payment received, invoice is now fully paid";
                        }
                        else
                        {
                            AmountPaid += payment.Amount;
                            Payments.Add(payment);
                            responseMessage = "another partial payment received, still not fully paid";
                        }
                    }
                }
                else
                {
                    if (payment.Amount > Amount)
                    {
                        responseMessage = "the payment is greater than the invoice amount";
                    }
                    else if (Amount == payment.Amount)
                    {
                        AmountPaid = payment.Amount;
                        Payments.Add(payment);
                        responseMessage = "invoice is now fully paid";
                    }
                    else
                    {
                        AmountPaid = payment.Amount;
                        Payments.Add(payment);
                        responseMessage = "invoice is now partially paid";
                    }
                }
            }

            return responseMessage;
        }
    }
}