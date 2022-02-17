using RefactorThis.Domain.PaymentProcessor.Validators;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.PaymentProcessor
{
	public class InvoicePaymentProcessor
	{
		private readonly InvoiceRepository _invoiceRepository;
        private readonly IPaymentProcessorValidator _paymentProcessorValidator;

		public InvoicePaymentProcessor( InvoiceRepository invoiceRepository, IPaymentProcessorValidator paymentProcessorValidator )
		{
			_invoiceRepository = invoiceRepository;
            _paymentProcessorValidator = paymentProcessorValidator;
        }

		public string ProcessPayment( Payment payment )
		{
			var inv = _invoiceRepository.GetInvoice( payment.Reference );

            var validationStatus = _paymentProcessorValidator.Validate(inv, payment);

            if (validationStatus.Success)
            {
                AddPayment(payment, inv);
            }

            inv.Save();

			return validationStatus.Message;
		}

        private static void AddPayment(Payment payment, Invoice inv)
        {
            inv.AmountPaid += payment.Amount;
            inv.Payments.Add(payment);
        }
    }
}