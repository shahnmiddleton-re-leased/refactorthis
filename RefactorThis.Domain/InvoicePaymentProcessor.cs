using Microsoft.Extensions.Logging;
using RefactorThis.Domain.Constants;
using RefactorThis.Persistence;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly ILogger<InvoicePaymentProcessor> _logger;
        private readonly InvoiceValidator _invoiceValidator;
        private readonly PaymentValidator _paymentValidator;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicePaymentProcessor(
            ILogger<InvoicePaymentProcessor> logger,
            InvoiceValidator invoiceProcessor,
            PaymentValidator paymentValidator,
            IInvoiceRepository invoiceRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _invoiceValidator = invoiceProcessor ?? throw new ArgumentNullException(nameof(invoiceProcessor));
            _paymentValidator = paymentValidator ?? throw new ArgumentNullException(nameof(paymentValidator));
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        }

        public string ProcessPayment(Payment payment)
        {
            _paymentValidator.PreValidatePayment(payment);
            
            using (_logger.BeginScope(new Dictionary<string, string> { ["ReferenceID"] = payment.Reference }))
            {
                return ProcessPaymentWithScopedLogging(payment);
            }
        }

        private string ProcessPaymentWithScopedLogging(Payment payment)
        {
            _logger.LogInformation("Processing payment");

            Invoice invoice = _invoiceRepository.GetInvoice(payment.Reference);

            _invoiceValidator.PreValidateInvoice(invoice);

            string paymentResult = PaymentResultMessage.NoPaymentNeeded;
            
            if (_invoiceValidator.DoesInvoiceRequireProcessing(invoice))
            {
                paymentResult = DeterminePaymentResult(invoice, payment);

                _invoiceRepository.SaveInvoice(invoice);
            }
            else
            {
                _logger.LogInformation("Invoice does not require payment");
            }

            return paymentResult;
        }

        private string DeterminePaymentResult(Invoice invoice, Payment payment)
        {
            string partialPaymentResult;
            
            bool isFirstPayment = !_invoiceValidator.DoesInvoiceHavingExistingPayments(invoice);

            if (_invoiceValidator.HasInvoiceBeenPaidInFull(invoice))
            {
                _logger.LogInformation("Invoice has already been paid - no payment required");
                partialPaymentResult = PaymentResultMessage.InvoiceAlreadyFullyPaid;
            }
            else if (_invoiceValidator.DoesPaymentExceedRemainingBalance(invoice, payment))
            {
                _logger.LogInformation("Payment of {Payment} exceeds the remaining balance {RemainingBalance}", payment.Amount, invoice.RemainingBalance);
                partialPaymentResult = PaymentResultMessage.PaymentExceedsRemainingBalance(isFirstPayment);
            }
            else
            {
                partialPaymentResult = MakePaymentToInvoice(invoice, payment, isFirstPayment);
            }

            return partialPaymentResult;
        }

        private string MakePaymentToInvoice(Invoice invoice, Payment payment, bool isFirstPayment)
        {
            invoice.Payments.Add(payment);

            _logger.LogInformation("Payment of {Amount} has been paid", payment.Amount);

            if (_invoiceValidator.HasInvoiceBeenPaidInFull(invoice))
            {
                _logger.LogInformation("Invoice has been paid in full.");
                return PaymentResultMessage.PaymentComplete(isFirstPayment);
            }
            else
            {
                _logger.LogInformation("Balance of {RemainingBalance} is still outstanding", invoice.RemainingBalance);
                return PaymentResultMessage.PaymentReceived(isFirstPayment);
            }
        }
    }
}
