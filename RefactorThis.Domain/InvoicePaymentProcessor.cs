using System;
using System.Linq;
using RefactorThis.Persistence;
using RefactorThis.Persistence.Interfaces;
using RefactorThis.Persistence.Model;

namespace RefactorThis.Domain
{
	public class InvoicePaymentProcessor
	{
        private readonly IFinancialsService _financialsService;

        public InvoicePaymentProcessor(IFinancialsService financialsService)
		{
            this._financialsService = financialsService;
		}

        /// <summary>
        /// ProcessPayment
        /// </summary>
        /// <param name="payment">payment</param>
        /// <returns>Payment response message</returns>
        public string ProcessPayment(Payment payment)
        {
            try
            {
                // Argument null check for payment.
                if (payment is null) { throw new ArgumentNullException(paramName: nameof(payment)); }

                // Get invoice by payment reference.
                var invoice = _financialsService.GetInvoice(payment.Reference);

                // Invoice null reference check.
                if (invoice is null) { throw new InvalidOperationException("There is no invoice matching this payment."); }
                else
                {
                    // When invoice amount is 0;
                    if (invoice.Amount == 0)
                    {
                        // No payments attached for this invoice;
                        if (invoice.Payments is null || invoice.Payments.Count == 0) { return "No payment needed."; }
                        else { throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments."); }
                    }
                    else
                    {
                        // When there are payments attached with this invoice.
                        if (!(invoice.Payments is null) && (invoice.Payments.Count > 0))
                        {

                            var invoicePaymentsSum = invoice.Payments.Sum(x => x.Amount);  // Invoice payments sum.
                            var invRemainingAmount = invoice.Amount - invoice.AmountPaid;

                            if (invoicePaymentsSum != 0 && invoice.Amount == invoicePaymentsSum)    // When invoice amount and invoice payments sum are equal.
                            {
                                return "Invoice was already fully paid.";
                            }
                            else if (invoicePaymentsSum != 0 && payment.Amount > invRemainingAmount) // When current payment amount is greater than invoice remaining amount.
                            {
                                return $"The payment {payment.Amount} is greater than the partial amount remaining {invRemainingAmount}";
                            }
                            else if (payment.Amount == invRemainingAmount)  // When current payment amount is equal to invoice remaining amount.
                            {
                                AddPaymentToInvoice(invoice, payment);
                                return $"Final partial payment {payment.Amount} received, invoice is now fully paid.";
                            }
                            else
                            {
                                // Another partial payment received.
                                AddPaymentToInvoice(invoice, payment);
                                return $"Another partial payment {payment.Amount} received, still not fully paid";
                            }
                        }
                        else
                        {
                            // When invoice amount is less than payment amount.
                            if (invoice.Amount < payment.Amount)
                            {
                                return $"The payment {payment.Amount} is greater than the invoice amount";
                            }
                            else if (invoice.Amount == payment.Amount) // When invoice amount equal payment amount.
                            {
                                AddPaymentToInvoice(invoice, payment);
                                return "Invoice is now fully paid.";
                            }
                            else
                            {
                                // Partial payment received, adding payment to invoice.
                                AddPaymentToInvoice(invoice, payment);
                                return "Invoice is now partially paid.";
                            }
                        }
                    }
                }
            }
            catch { throw; }
        }

        private void AddPaymentToInvoice(Invoice invoice, Payment payment)
        {
            try
            {
                invoice.AmountPaid += payment.Amount;
                invoice.Payments.Add(payment);
                _financialsService.SaveInvoice(invoice);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}