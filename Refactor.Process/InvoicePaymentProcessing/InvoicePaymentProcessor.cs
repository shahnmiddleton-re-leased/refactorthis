using RefactorThis.Domain;
using RefactorThis.Domain.Model;
using RefactorThis.Domain.Services;
using System;
using System.Collections.Generic;

namespace Refactor.Process.InvoicePaymentProcessing
{
    public class InvoicePaymentProcessor : AbstractInvoicePaymentProcessor
    {
        private Dictionary<string, string> _messages;

        protected override void InitializeMessages()
        {
            _messages = new Dictionary<string, string>();
            _messages[MSG_NO_MATCHING_INVOICE_PAYMENT] = "There is no invoice matching this payment";
            _messages[MSG_NO_PAYMENT_NEED] = "no payment needed";
            _messages[MSG_INVOICE_INVALID_STAT_AMNT_0_HAS_PAYMENTS] = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
            _messages[MSG_INVOICE_PAID_FULLY] = "invoice was already fully paid";
            _messages[MSG_PAYMENT_LARGER_THAN_REMAIN] = "the payment is greater than the partial amount remaining";
            _messages[MSG_FINAL_PAY_INV_FULLY_PAID] = "final partial payment received, invoice is now fully paid";
            _messages[MSG_PAY_RCVD_NOT_FULLY_PAID] = "another partial payment received, still not fully paid";
            _messages[MSG_PAY_LARGER_THAN_INV_AMT] = "the payment is greater than the invoice amount";
            _messages[MSG_INV_FULLY_PAID] = "invoice is now fully paid";
            _messages[MSG_INV_PARTLY_PAID] = "invoice is now partially paid";
        }

        public InvoicePaymentProcessor(IRepository<Invoice> invoiceRepository) : base(invoiceRepository)
        {
        }

        public override string ProcessPayment(Payment payment)
        {
            try
            {
                return _messages[base.ProcessPayment(payment)];
            }
            catch(InvalidOperationException invEx)
            {
                throw new InvalidOperationException(_messages[invEx.Message]);
            }
        }
    }
}
