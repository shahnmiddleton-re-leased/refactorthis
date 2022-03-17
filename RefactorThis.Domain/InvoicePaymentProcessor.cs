using System;
using System.Diagnostics;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            // Check for null first so we don't process any more
            if (inv == null) throw new InvalidOperationException("There is no invoice matching this payment");

            if (inv.Amount == 0)
            {
                if (!inv.Payments.Any())
                    return "No payment needed";

                throw new InvalidOperationException(
                    "The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }

            // If there is nothing outstanding, return it here before 
            // processing further
            if (inv.AmountOutstanding == 0) return "Invoice was already fully paid";

            // Get the payments that were made
            // If there are no payments, this will be 0
            var paymentsMade = inv.Payments.Count();

            // Using switch so we can add more logic later
            switch (payment.Amount - inv.AmountOutstanding)
            {
                case 0: // The payment is the same as the amount outstanding
                    // Update and save the invoice
                    inv.AmountPaid = payment.Amount;
                    inv.Payments.Add(payment);
                    inv.Save();


                    return paymentsMade == 0 // We had no payments before this...
                        ? "Invoice is now partially paid"
                        : "Final partial payment received, invoice is now fully paid";
                    break;

                case < 0: // The payment made is less than the amount outstanding
                {
                    // Update and save the invoice
                    inv.AmountPaid = payment.Amount;
                    inv.Payments.Add(payment);
                    inv.Save();

                    // Return a value depending on how many payments were made before this
                    return paymentsMade == 0
                        ? "Invoice is now partially paid"
                        : "Another partial payment received, still not fully paid";
                }
                    break;

                case > 0: // The payment is more than what's outstanding
                    return paymentsMade == 0
                        ? "The payment is greater than the invoice amount"
                        : "The payment is greater than the partial amount remaining";
            }
            //
            // if (inv.Payments.Any())
            // {
            //     if (paymentsMade != 0 && inv.AmountOutstanding == 0)
            //     {
            //         responseMessage = "Invoice was already fully paid";
            //     }
            //     else if (paymentsMade != 0 && payment.Amount > inv.AmountOutstanding)
            //     {
            //         responseMessage = "the payment is greater than the partial amount remaining";
            //     }
            //     else
            //     {
            //         if (inv.AmountOutstanding == payment.Amount)
            //         {
            //             inv.AmountPaid += payment.Amount;
            //             inv.Payments.Add(payment);
            //             responseMessage = "final partial payment received, invoice is now fully paid";
            //         }
            //         else
            //         {
            //             inv.AmountPaid += payment.Amount;
            //             inv.Payments.Add(payment);
            //             responseMessage = "another partial payment received, still not fully paid";
            //         }
            //     }
            // }
            //
            // else
            // {
            //     if (payment.Amount > inv.Amount)
            //     {
            //         responseMessage = "the payment is greater than the invoice amount";
            //     }
            //     else if (inv.Amount == payment.Amount)
            //     {
            //         inv.AmountPaid = payment.Amount;
            //         inv.Payments.Add(payment);
            //         responseMessage = "invoice is now fully paid";
            //     }
            //     else
            //     {
            //         inv.AmountPaid = payment.Amount;
            //         inv.Payments.Add(payment);
            //         responseMessage = "invoice is now partially paid";
            //     }
            // }
            //
            //
            // inv.Save();
            //
            // return responseMessage;
        }
    }
}