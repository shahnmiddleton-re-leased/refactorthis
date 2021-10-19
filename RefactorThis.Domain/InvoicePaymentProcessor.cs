using RefactorThis.Persistence;
using RefactorThis.Validation;

namespace RefactorThis.Domain
{
    public class InvoicePaymentProcessor
    {
        private readonly InvoiceRepository _invoiceRepository;
        public string responseMessage = "";
        public InvoicePaymentProcessor(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        // Method for exception handling


        // ---- start of Processing
        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);
            var InvoiceValidation = new InvoiceValidation();
            if (InvoiceValidation.CheckInvoiceValidity(inv, out responseMessage))
            {
                // ---- process invoice data
                ProcessInvoice(payment, inv, InvoiceValidation);
            }
            inv.Save();
            return responseMessage;
        }


        public void ProcessInvoice(Payment payment, Invoice inv, InvoiceValidation InvoiceValidation)
        {
            if (InvoiceValidation.isExistingInvoice(inv))
            {
                ProcessExistingInvoice(payment, inv, InvoiceValidation);
            }
            else
            {
                ProcessNewInvoice(payment, inv, InvoiceValidation);
            }
        }

        private void ProcessExistingInvoice(Payment payment, Invoice inv, InvoiceValidation InvoiceValidation)
        {
            // check payment status this locks the processing the payment from negative scenarios
            if (InvoiceValidation.ValidatePayment(payment, inv, Constants.partialPayment, out responseMessage))
            {
                //process the payment
                ProcessPaymentMethod(payment, inv, Constants.partialPayment);
            }

        }

        private void ProcessNewInvoice(Payment payment, Invoice inv, InvoiceValidation InvoiceValidation)
        {
            // check payment status this locks the processing the payment from negative scenarios
            if (!InvoiceValidation.ValidatePayment(payment, inv, Constants.initialPayment, out responseMessage))
            {
                ProcessPaymentMethod(payment, inv, Constants.initialPayment);
            }
        }

        // ---- logic and conditions


        private void ProcessPaymentMethod(Payment payment, Invoice inv, string paymentMethod)
        {
            // ---- process payment based on initial or partial payment
            _ = paymentMethod == Constants.initialPayment ? inv.AmountPaid = payment.Amount : inv.AmountPaid += payment.Amount;
            inv.Payments.Add(payment);
        }

    }
}