using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace RefactorThis.Domain.Model.Invoices
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

        public decimal PaymentsRunningTotal => Payments.Sum(x => x.Amount);
        public bool HasPayments => Payments.Any();

        private readonly List<Payment> _payments = new List<Payment>();
        public IReadOnlyList<Payment> Payments => _payments;
        
        public Result AddPayment(Payment payment)
        {
            var validationResult = new InvoicePaymentValidator().IsValid(this, payment);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            string successMessage;
            if (HasPayments)
            {
                successMessage = IsFinalPartialPaymentReceived(payment) 
                    ? "final partial payment received, invoice is now fully paid" 
                    : "another partial payment received, still not fully paid";

                AmountPaid += payment.Amount;
            }
            else
            {
                successMessage = IsFullPaymentReceived(payment) 
                    ? "invoice is now fully paid" 
                    : "invoice is now partially paid";
                    
                AmountPaid = payment.Amount;
            }

            _payments.Add(payment);
            return Result.Ok().WithSuccess(successMessage);
        }

        private bool IsFullPaymentReceived(Payment payment)
        {
            return Amount == payment.Amount;
        }

        private bool IsFinalPartialPaymentReceived(Payment payment)
        {
            return Amount - AmountPaid == payment.Amount;
        }
    }
}