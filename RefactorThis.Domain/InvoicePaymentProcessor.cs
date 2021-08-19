using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor()
        {
            _invoiceRepository = InvoiceRepository.GetInvoiceRepository();
        }

        /// <summary>
        /// ProcessPayment accepts valid payment object and add the payment to the invoice specified by the reference.
        /// </summary>
        /// <param name="payment">Payment object containing the payment amount and reference</param>
        /// <returns>
        ///   string: a descriptive string, which elaborates the ProcessPayment status.
        /// </returns>
        public string ProcessPayment(Payment payment)
        {
            if (payment == null)
            {
                return "payment is null";
            }
            
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            string responseMessage;

            if (inv == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }
            
            bool isInitialPayment = !inv.GetPayments().Any();
            var addPaymentStatus = inv.AddPayment(payment);
            
            switch (addPaymentStatus)
            {
                case Invoice.AddPaymentStatus.FailInvalidAmount:
                    responseMessage = "payment amount in invalid";
                    break;
                case Invoice.AddPaymentStatus.FailOverPayment:
                    responseMessage = isInitialPayment
                        ? "the payment is greater than the invoice amount"
                        : "the payment is greater than the partial amount remaining";
                    break;
                case Invoice.AddPaymentStatus.FailFullyPaidInvoice:
                    responseMessage = isInitialPayment 
                        ? "no payment needed" : 
                        "invoice was already fully paid";
                    break;
                case Invoice.AddPaymentStatus.SuccessPartiallyPaid:
                    responseMessage = isInitialPayment
                        ? "invoice is now partially paid"
                        : "another partial payment received, still not fully paid";
                    inv.Save();
                    break;
                case Invoice.AddPaymentStatus.SuccessFullyPaid:
                    responseMessage = isInitialPayment
                        ? "invoice is now fully paid"
                        : "final partial payment received, invoice is now fully paid";
                    inv.Save();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return responseMessage;
        }
    }
}